using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace InterfaceBuilder
{
    public class CompositeCreator
    {
        public IPool<T> Create<T>() where T : class
        {
            AssemblyName assemblyName = new AssemblyName
            {
                Name = "CompositeAssembly"
            };

            var poolType = typeof(IPool<>);
            var genericPoolType = poolType.MakeGenericType(typeof(T));

            AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(
                assemblyName,
                AssemblyBuilderAccess.Run);

            ModuleBuilder myModule = assemblyBuilder.DefineDynamicModule("CompositeAssembly");

            var name = typeof(T).Name;
            if (name[0] == 'I')
                name = name.Substring(1, name.Length - 1);

            TypeBuilder typeBuilder = myModule.DefineType($"Composite{name}",
                TypeAttributes.Public);
            typeBuilder.AddInterfaceImplementation(typeof(T));
            typeBuilder.AddInterfaceImplementation(genericPoolType);

            FieldBuilder fieldBuilder = typeBuilder.DefineField("_items", typeof(List<T>), FieldAttributes.Public);

            var poolMethods = genericPoolType.GetMethods();
            foreach (var method in poolMethods)
            {
                //todo check parameters
                if (method.Name == "Add")
                {
                    var addMethod = CreateAddMethod(typeBuilder, fieldBuilder, typeof(T));
                    typeBuilder.DefineMethodOverride(addMethod, method);
                }
                else if (method.Name == "Remove")
                {
                    var addMethod = CreateRemoveMethod(typeBuilder, fieldBuilder, typeof(T));
                    typeBuilder.DefineMethodOverride(addMethod, method);
                }
            }

            var methods = typeof(T).GetMethods();
            foreach (var method in methods)
            {
                var methodBuilder = CreateInterfaceMethod(typeBuilder, method, fieldBuilder);
                typeBuilder.DefineMethodOverride(methodBuilder, method);
            }

            Type myType = typeBuilder.CreateType();
            object instance = Activator.CreateInstance(myType!);
            instance!.GetType().GetField("_items")!.SetValue(instance, new List<T>());
            return instance as IPool<T>;
        }

        private MethodBuilder CreateAddMethod(TypeBuilder typeBuilder, FieldBuilder items, Type generic)
        {
            MethodBuilder builder = typeBuilder.DefineMethod(
                $"Add",
                MethodAttributes.Public | MethodAttributes.Virtual,
                typeof(void),
                new Type[] {generic});
            
            FieldInfo field = items;
            var add = field.FieldType.GetMethod("Add");

            ILGenerator lout = builder.GetILGenerator();
            lout.Emit(OpCodes.Ldarg_0);
            lout.Emit(OpCodes.Ldfld, field);
            lout.Emit(OpCodes.Ldarg_1);
            lout.Emit(OpCodes.Callvirt, add!);
            
            lout.Emit(OpCodes.Ret);

            return builder;
        }
        
        private MethodBuilder CreateRemoveMethod(TypeBuilder typeBuilder, FieldBuilder items, Type generic)
        {
            MethodBuilder builder = typeBuilder.DefineMethod(
                $"Remove",
                MethodAttributes.Public | MethodAttributes.Virtual,
                typeof(void),
                new Type[] {generic});
            
            FieldInfo field = items;
            var remove = field.FieldType.GetMethod("Remove");

            ILGenerator lout = builder.GetILGenerator();
            lout.Emit(OpCodes.Ldarg_0);
            lout.Emit(OpCodes.Ldfld, field);
            lout.Emit(OpCodes.Ldarg_1);
            lout.Emit(OpCodes.Callvirt, remove!);
            lout.Emit(OpCodes.Pop);

            lout.Emit(OpCodes.Ret);

            return builder;
        }

        private MethodBuilder CreateInterfaceMethod(TypeBuilder typeBuilder, MethodInfo method, FieldBuilder items)
        {
            MethodBuilder builder = typeBuilder.DefineMethod(
                $"{method.Name}",
                MethodAttributes.Public | MethodAttributes.Virtual |  MethodAttributes.HideBySig,
                typeof(Result),
                new Type[] {});

            ILGenerator lout = builder.GetILGenerator();
            var jump = lout.DefineLabel();
            var jump1 = lout.DefineLabel();
            var jump2 = lout.DefineLabel();
            var i = lout.DeclareLocal(typeof(Int32));
            var v1 = lout.DeclareLocal(typeof(bool));
            var v2 = lout.DeclareLocal(typeof(Result));
            
            FieldInfo field = items;

            var getItem = field.FieldType.GetMethod("get_Item");
            var getCount = field.FieldType.GetMethod("get_Count");
            
            lout.Emit(OpCodes.Ldc_I4_0);
            lout.Emit(OpCodes.Stloc_0);

            lout.Emit(OpCodes.Br_S, jump);

            lout.MarkLabel(jump1);
            lout.Emit(OpCodes.Ldarg_0);
            lout.Emit(OpCodes.Ldfld, field);
            lout.Emit(OpCodes.Ldloc_0);
            lout.Emit(OpCodes.Callvirt, getItem!);
            lout.Emit(OpCodes.Callvirt, method);
            lout.Emit(OpCodes.Pop);

            lout.Emit(OpCodes.Nop);

            lout.Emit(OpCodes.Ldloc_0);
            lout.Emit(OpCodes.Ldc_I4_1);
            lout.Emit(OpCodes.Add);
            lout.Emit(OpCodes.Stloc_0);

            lout.MarkLabel(jump);
            lout.Emit(OpCodes.Ldloc_0);
            lout.Emit(OpCodes.Ldarg_0);
            lout.Emit(OpCodes.Ldfld, field);
            lout.Emit(OpCodes.Callvirt, getCount!);
            lout.Emit(OpCodes.Clt);
            lout.Emit(OpCodes.Stloc_1);

            lout.Emit(OpCodes.Ldloc_1);
            lout.Emit(OpCodes.Brtrue_S, jump1);
            
            
            lout.Emit(OpCodes.Ldc_I4_0);
            lout.Emit(OpCodes.Stloc_2);
            lout.Emit(OpCodes.Br_S, jump2);
            
            lout.MarkLabel(jump2);
            lout.Emit(OpCodes.Ldloc_2);
            lout.Emit(OpCodes.Ret);

            return builder;
        }
    }
}