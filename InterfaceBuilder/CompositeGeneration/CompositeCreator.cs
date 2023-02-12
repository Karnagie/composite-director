using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace InterfaceBuilder.CompositeGeneration
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

            FieldBuilder fieldBuilder = typeBuilder.DefineField("Items", typeof(List<T>), FieldAttributes.Public);
            CreateGetItemsMethod(typeBuilder, fieldBuilder, typeof(T));
            
            
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
                var intern = CreateInternalCopy(typeBuilder, method, typeof(T));
                if(method.GetParameters().Length == 0)
                    CreateInterfaceMethod(typeBuilder, method, fieldBuilder, intern,
                        typeof(T));
                else
                {
                    List<KeyValuePair<Type, string>> args = new ();
                    foreach (var parameter in method.GetParameters())
                    {
                        args.Add(new KeyValuePair<Type, string>(parameter.ParameterType, parameter.Name));
                    }
                    
                    var internalCopyCreator = new InternalClassCreator();
                    var internalCopy = internalCopyCreator.Create(myModule, typeBuilder, intern, 
                        fieldBuilder.FieldType, args.ToArray());
                    
                    CreateInterfaceMethodWithParams(typeBuilder, method, typeof(T), args.ToArray(), internalCopy);
                }
            }

            Type myType = typeBuilder.CreateType();
            object instance = Activator.CreateInstance(myType!);
            instance!.GetType().GetField("Items")!.SetValue(instance, new List<T>());
            return instance as IPool<T>;
        }

        private MethodInfo CreateGetItemsMethod(TypeBuilder typeBuilder, FieldBuilder items, Type generic)
        {
            var poolType = typeof(List<>);
            var genericPoolType = poolType.MakeGenericType(generic);
            
            MethodBuilder builder = typeBuilder.DefineMethod(
                $"get_Items",
                MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Final |
                MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.SpecialName,
                genericPoolType,
                new Type[] {});
            FieldInfo field = items;
            
            ILGenerator lout = builder.GetILGenerator();
            lout.Emit(OpCodes.Ldarg_0);
            lout.Emit(OpCodes.Ldfld, field);
            // IL_0000: ldarg.0      // this
            // IL_0001: ldfld        class [System.Collections]System.Collections.Generic.List`1<class ITest> TestComposite::'<Items>k__BackingField'
            // IL_0006: ret
            
            lout.Emit(OpCodes.Ret);

            return builder;
        }

        private MethodInfo CreateInternalCopy(TypeBuilder typeBuilder, MethodInfo method, Type generic)
        {
            var parameters = new List<Type>();
            parameters.Add(generic.MakeArrayType());
            foreach (var parameter in method.GetParameters())
            {
                parameters.Add(parameter.ParameterType);
            }
            
            MethodBuilder builder = typeBuilder.DefineMethod(
                $"{method.Name}Internal",
                MethodAttributes.Public |  MethodAttributes.HideBySig,
                typeof(void),
                parameters.ToArray());

            ILGenerator lout = builder.GetILGenerator();
            var jump = lout.DefineLabel();
            var jump1 = lout.DefineLabel();
            
            var i = lout.DeclareLocal(typeof(Int32)); 
            var v1 = lout.DeclareLocal(typeof(bool));
            
            lout.Emit(OpCodes.Ldc_I4_0);
            lout.Emit(OpCodes.Stloc_0);
            lout.Emit(OpCodes.Br_S, jump);
            
            lout.MarkLabel(jump1);
            lout.Emit(OpCodes.Ldarg_1);
            lout.Emit(OpCodes.Ldloc_0);
            lout.Emit(OpCodes.Ldelem_Ref);
            for (int j = 1; j < parameters.Count; j++)
            {
                switch (j)
                {
                    case 1: 
                        lout.Emit(OpCodes.Ldarg_2);
                        break;
                    case 2: 
                        lout.Emit(OpCodes.Ldarg_3);
                        break;
                    case 3:
                        lout.Emit(OpCodes.Ldarg_S, j);
                        break;
                }
            }
            
            lout.Emit(OpCodes.Callvirt, method);
            lout.Emit(OpCodes.Pop);
            
            lout.Emit(OpCodes.Ldloc_0);
            lout.Emit(OpCodes.Ldc_I4_1);
            lout.Emit(OpCodes.Add);
            lout.Emit(OpCodes.Stloc_0);
            
            lout.MarkLabel(jump);
            lout.Emit(OpCodes.Ldloc_0);
            lout.Emit(OpCodes.Ldarg_1);
            lout.Emit(OpCodes.Ldlen);
            lout.Emit(OpCodes.Conv_I4);
            lout.Emit(OpCodes.Clt);
            lout.Emit(OpCodes.Stloc_1);
            
            lout.Emit(OpCodes.Ldloc_1);
            lout.Emit(OpCodes.Brtrue_S, jump1);
            
            lout.Emit(OpCodes.Ret);
            
            return builder;
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

        private MethodBuilder CreateInterfaceMethod(TypeBuilder typeBuilder, MethodInfo method, FieldBuilder items, 
            MethodInfo internalCopy, Type generic)
        {
            MethodBuilder builder = typeBuilder.DefineMethod(
                $"{method.Name}",
                MethodAttributes.Public | MethodAttributes.Virtual |  MethodAttributes.HideBySig,
                typeof(Result),
                new Type[] {});

            ILGenerator lout = builder.GetILGenerator();
            
            var actionType = typeof(Action<>);
            var genericArray = generic.MakeArrayType();
            var genericActionType = actionType.MakeGenericType(genericArray);
            
            var compType = typeof(IPool<>);
            var genericCompType = compType.MakeGenericType(generic);
            
            var comp = lout.DeclareLocal(genericCompType);
            var func = lout.DeclareLocal(genericActionType);
            var v2 = lout.DeclareLocal(typeof(Result));
            
            var intern = internalCopy;

            var groupMethod = typeof(CompositeHelper).GetMethod("Group").MakeGenericMethod(generic);
            var intern1 = genericActionType.GetConstructors()[0];
            
            var jump = lout.DefineLabel();
            
            lout.Emit(OpCodes.Ldarg_0);
            lout.Emit(OpCodes.Stloc_0);
            
            lout.Emit(OpCodes.Ldarg_0);
            lout.Emit(OpCodes.Ldftn, intern);
            lout.Emit(OpCodes.Newobj, intern1);
            lout.Emit(OpCodes.Stloc_1);
            
            lout.Emit(OpCodes.Ldloc_1);
            lout.Emit(OpCodes.Ldloc_0);
            lout.Emit(OpCodes.Call, groupMethod!);

            lout.Emit(OpCodes.Ldc_I4_0);
            lout.Emit(OpCodes.Stloc_2);
            lout.Emit(OpCodes.Br_S, jump);

            lout.MarkLabel(jump);
            lout.Emit(OpCodes.Ldloc_2);
            lout.Emit(OpCodes.Ret);

            return builder;
        }
        
        private MethodBuilder CreateInterfaceMethodWithParams(TypeBuilder typeBuilder, MethodInfo method, 
            Type generic, KeyValuePair<Type, string>[] args, Type internalClass)
        {
            MethodBuilder builder = typeBuilder.DefineMethod(
                $"{method.Name}",
                MethodAttributes.Public | MethodAttributes.Virtual |  MethodAttributes.HideBySig,
                typeof(Result),
                args.Select((pair => pair.Key)).ToArray());

            ILGenerator lout = builder.GetILGenerator();
            var composite = lout.DeclareLocal(internalClass);
            var v1 = lout.DeclareLocal(typeof(Result));

            var actionType = typeof(Action<>);
            var genericArray = generic.MakeArrayType();
            var genericActionType = actionType.MakeGenericType(genericArray);
            var genericActionTypeConstructor = genericActionType.GetConstructors()[0];

            var groupMethod = typeof(CompositeHelper).GetMethod("Group").MakeGenericMethod(generic);
            
            ConstructorInfo internalClassConstructor = internalClass.GetConstructors()[0];
            FieldInfo _this = internalClass.GetField("_this");
            MethodInfo b__0 = internalClass.GetMethod("b__0");

            lout.Emit(OpCodes.Newobj, internalClassConstructor);
            lout.Emit(OpCodes.Stloc_0);
            lout.Emit(OpCodes.Ldloc_0);
            lout.Emit(OpCodes.Ldarg_0);
            lout.Emit(OpCodes.Stfld, _this!);
            for (int i = 0; i < args.Length; i++)
            {
                FieldInfo field = internalClass.GetField(args[i].Value);
                lout.Emit(OpCodes.Ldloc_0);
                switch (i)
                {
                    case 0:
                        lout.Emit(OpCodes.Ldarg_1);
                        break;
                    case 1:
                        lout.Emit(OpCodes.Ldarg_2);
                        break;
                    case 2:
                        lout.Emit(OpCodes.Ldarg_3);
                        break;
                    default:
                        lout.Emit(OpCodes.Ldarg_S, (byte)i);
                        break;
                }
                lout.Emit(OpCodes.Stfld, field!);
            }
            
            lout.Emit(OpCodes.Ldloc_0);
            lout.Emit(OpCodes.Ldftn, b__0!);
            lout.Emit(OpCodes.Newobj, genericActionTypeConstructor);
            lout.Emit(OpCodes.Ldarg_0);
            lout.Emit(OpCodes.Call, groupMethod!);
            
            lout.Emit(OpCodes.Ldc_I4_0);
            lout.Emit(OpCodes.Stloc_1);
            lout.Emit(OpCodes.Ldloc_1);
            
            lout.Emit(OpCodes.Ret);

            return builder;
        }
        
        private MethodBuilder CreateInterfaceMethodWithParams(TypeBuilder typeBuilder, MethodInfo method, FieldBuilder items, 
            MethodInfo internalCopy, Type generic, Type[] args, string test)
        {
            MethodBuilder builder = typeBuilder.DefineMethod(
                $"{method.Name}",
                MethodAttributes.Public | MethodAttributes.Virtual |  MethodAttributes.HideBySig,
                typeof(Result),
                args);

            ILGenerator lout = builder.GetILGenerator();
            // var jump = lout.DefineLabel();
            // var jump1 = lout.DefineLabel();
            // var jump2 = lout.DefineLabel();
            // var i = lout.DeclareLocal(typeof(Int32));
            // var v1 = lout.DeclareLocal(typeof(bool));
            var composite = lout.DeclareLocal(generic);
            var v1 = lout.DeclareLocal(typeof(Result));
            
            //FieldInfo field = items;

            //var getItem = field.FieldType.GetMethod("get_Item");
            //var getCount = field.FieldType.GetMethod("get_Count");
            var intern = internalCopy;

            var actionType = typeof(Action<>);
            var genericArray = generic.MakeArrayType();
            var genericActionType = actionType.MakeGenericType(genericArray);

            var groupMethod = typeof(CompositeHelper).GetMethod("Group");
            
            lout.Emit(OpCodes.Newobj, generic);
            lout.Emit(OpCodes.Stloc_0);
            lout.Emit(OpCodes.Ldloc_0);
            lout.Emit(OpCodes.Ldarg_0);
            lout.Emit(OpCodes.Stfld, generic);
            lout.Emit(OpCodes.Ldloc_0);
            lout.Emit(OpCodes.Ldarg_0);
            lout.Emit(OpCodes.Stfld, generic);
            lout.Emit(OpCodes.Ldloc_0);
            lout.Emit(OpCodes.Ldarg_2);
            lout.Emit(OpCodes.Stfld, generic);

            // IL_0000: newobj       instance void TestComposite/'<>c__DisplayClass7_0'::.ctor()
            // IL_0005: stloc.0      // 'CS$<>8__locals0'
            // IL_0006: ldloc.0      // 'CS$<>8__locals0'
            // IL_0007: ldarg.0      // this
            // IL_0008: stfld        class TestComposite TestComposite/'<>c__DisplayClass7_0'::'<>4__this'
            // IL_000d: ldloc.0      // 'CS$<>8__locals0'
            // IL_000e: ldarg.1      // 'value'
            // IL_000f: stfld        string TestComposite/'<>c__DisplayClass7_0'::'value'

            // IL_0000: newobj       instance void TestComposite/'<>c__DisplayClass9_0'::.ctor()
            // IL_0005: stloc.0      // 'CS$<>8__locals0'
            // IL_0006: ldloc.0      // 'CS$<>8__locals0'
            // IL_0007: ldarg.0      // this
            // IL_0008: stfld        class TestComposite TestComposite/'<>c__DisplayClass9_0'::'<>4__this'
            // IL_000d: ldloc.0      // 'CS$<>8__locals0'
            // IL_000e: ldarg.1      // 'value'
            // IL_000f: stfld        string TestComposite/'<>c__DisplayClass9_0'::'value'
            // IL_0014: ldloc.0      // 'CS$<>8__locals0'
            // IL_0015: ldarg.2      // value1
            // IL_0016: stfld        int32 TestComposite/'<>c__DisplayClass9_0'::value1
            
            //
            
            
            
            // // [58 5 - 58 6]
            // IL_001b: nop
            //
            // // [59 9 - 62 18]
            // IL_001c: ldloc.0      // 'CS$<>8__locals0'
            // IL_001d: ldftn        instance void TestComposite/'<>c__DisplayClass9_0'::'<FooWithTwoArgs>b__0'(class ITest[])
            // IL_0023: newobj       instance void class [System.Runtime]System.Action`1<class ITest[]>::.ctor(object, native int)
            // IL_0028: ldarg.0      // this
            // IL_0029: call         void CompositeHelper::Group<class ITest>(class [System.Runtime]System.Action`1<!!0/*class ITest*/[]>, class IPool`1<!!0/*class ITest*/>)
            // IL_002e: nop
            //
            // // [64 9 - 64 24]
            // IL_002f: ldc.i4.0
            // IL_0030: stloc.1      // V_1
            // IL_0031: br.s         IL_0033
            //
            // // [65 5 - 65 6]
            // IL_0033: ldloc.1      // V_1
            // IL_0034: ret
            
            // lout.Emit(OpCodes.Ldc_I4_0);
            // lout.Emit(OpCodes.Stloc_0);
            //
            // lout.Emit(OpCodes.Br_S, jump);
            //
            // lout.MarkLabel(jump1);
            // lout.Emit(OpCodes.Ldarg_0);
            // lout.Emit(OpCodes.Ldfld, field);
            // lout.Emit(OpCodes.Ldloc_0);
            // lout.Emit(OpCodes.Callvirt, getItem!);
            // lout.Emit(OpCodes.Callvirt, method);
            // lout.Emit(OpCodes.Pop);
            //
            // lout.Emit(OpCodes.Nop);
            //
            // lout.Emit(OpCodes.Ldloc_0);
            // lout.Emit(OpCodes.Ldc_I4_1);
            // lout.Emit(OpCodes.Add);
            // lout.Emit(OpCodes.Stloc_0);
            //
            // lout.MarkLabel(jump);
            // lout.Emit(OpCodes.Ldloc_0);
            // lout.Emit(OpCodes.Ldarg_0);
            // lout.Emit(OpCodes.Ldfld, field);
            // lout.Emit(OpCodes.Callvirt, getCount!);
            // lout.Emit(OpCodes.Clt);
            // lout.Emit(OpCodes.Stloc_1);
            //
            // lout.Emit(OpCodes.Ldloc_1);
            // lout.Emit(OpCodes.Brtrue_S, jump1);
            //
            //
            // lout.Emit(OpCodes.Ldc_I4_0);
            // lout.Emit(OpCodes.Stloc_2);
            // lout.Emit(OpCodes.Br_S, jump2);
            //
            // lout.MarkLabel(jump2);
            // lout.Emit(OpCodes.Ldloc_2);
            // lout.Emit(OpCodes.Ret);

            return builder;
        }
    }
}