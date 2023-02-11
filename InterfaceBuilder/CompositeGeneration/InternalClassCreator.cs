using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace InterfaceBuilder.CompositeGeneration
{
    public class InternalClassCreator
    {
        public Type Create(ModuleBuilder myModule, TypeBuilder compositeType, MethodInfo internalMethod, Type itemsType, params KeyValuePair<Type, string>[] parameters)
        {
            // AssemblyName assemblyName = new AssemblyName
            // {
            //     Name = "InternalClassesAssembly"
            // };

            // AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(
            //     assemblyName,
            //     AssemblyBuilderAccess.Run);

            //ModuleBuilder myModule = assemblyBuilder.DefineDynamicModule("InternalClassesAssembly");
            
            var name = $"Internal_{compositeType.Name}_{internalMethod.Name}";
            foreach (var parameter in parameters)
            {
                //todo replace to index
                name += $"_{parameter.Key.ToString()!.ToLower()}";
            }

            Console.WriteLine(name);
            
            //TypeBuilder typeBuilder = compositeType.DefineNestedType(name, TypeAttributes.NestedPublic);
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
            //object instance = Activator.CreateInstance(myType!);
            return myType;
        }

        private void CreateMethod(TypeBuilder typeBuilder, FieldInfo composite, MethodInfo internalMethod, Type itemsType, params FieldInfo[] parameters)
        {
            MethodBuilder builder = typeBuilder.DefineMethod(
                $"b__0",
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName,
                typeof(void),
                new [] {itemsType});
            
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