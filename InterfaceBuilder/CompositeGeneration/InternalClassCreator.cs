using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace InterfaceBuilder.CompositeGeneration
{
    public class InternalClassCreator
    {
        public object Create(TypeBuilder compositeType, MethodInfo internalMethod, Type itemsType, params KeyValuePair<Type, string>[] parameters)
        {
            AssemblyName assemblyName = new AssemblyName
            {
                Name = "InternalClassesAssembly"
            };

            AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(
                assemblyName,
                AssemblyBuilderAccess.Run);

            ModuleBuilder myModule = assemblyBuilder.DefineDynamicModule("InternalClassesAssembly");
            //compositeType.DefineNestedType()
            var name = $"{compositeType.ToString()!.ToLower()}{itemsType.ToString().ToLower()}";
            foreach (var parameter in parameters)
            {
                //todo replace to index
                name += $"{parameter.Key.ToString()!.ToLower()}{parameter.Value.ToLower()}";
            }

            TypeBuilder typeBuilder = myModule.DefineType($"Internal{name}",
                TypeAttributes.Public);
            
            FieldBuilder composite = typeBuilder.DefineField($"_this", 
                compositeType, FieldAttributes.Public);

            List<FieldBuilder> fields = new();
            foreach (var parameter in parameters)
            {
                FieldBuilder fieldBuilder = typeBuilder.DefineField($"{parameter.Value}", 
                    parameter.Key, FieldAttributes.Public);
                fields.Add(fieldBuilder);
            }
            
            // ReSharper disable once CoVariantArrayConversion
            CreateMethod(typeBuilder, composite, internalMethod, itemsType, fields.ToArray());

            Type myType = typeBuilder.CreateType();
            object instance = Activator.CreateInstance(myType!);
            // foreach (var parameter in parameters)
            // {
            //     instance!.GetType().GetField(parameter.Value)!.SetValue(instance, new List<T>());   
            // }
            return instance;
        }

        private void CreateMethod(TypeBuilder typeBuilder, FieldInfo composite, MethodInfo internalMethod, Type itemsType, params FieldInfo[] parameters)
        {
            MethodBuilder builder = typeBuilder.DefineMethod(
                $"b__0",
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName,
                typeof(void),
                new Type[] {itemsType});
            
            ILGenerator lout = builder.GetILGenerator();
            
            var items = lout.DeclareLocal(itemsType); 
            
            lout.Emit(OpCodes.Ldarg_0);
            lout.Emit(OpCodes.Ldfld, composite);
            lout.Emit(OpCodes.Ldarg_1);
            foreach (FieldInfo parameter in parameters)
            {
                lout.Emit(OpCodes.Ldarg_0);
                lout.Emit(OpCodes.Ldfld, parameter);
            }
            lout.Emit(OpCodes.Call, internalMethod);
            lout.Emit(OpCodes.Ret);
        }
    }
}