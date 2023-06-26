using System;
using System.Reflection;
using System.Reflection.Emit;

namespace InterfaceBuilder.CompositeGeneration
{
    public class InternalRemovingClassCreator
    {
        public Type Create(ModuleBuilder myModule, TypeBuilder compositeType, Type itemsType, MethodInfo remove)
        {
            var name = $"{compositeType.FullName}+Internal_Removing";
            
            TypeBuilder typeBuilder = myModule.DefineType(name,
                TypeAttributes.Public);
            FieldBuilder composite = typeBuilder.DefineField($"_this", 
                compositeType, FieldAttributes.Public);
            FieldBuilder item = typeBuilder.DefineField($"_item", 
                itemsType, FieldAttributes.Public);
            
            CreateMethod(typeBuilder, composite, item, remove);

            return typeBuilder.CreateType();
        }

        private void CreateMethod(TypeBuilder typeBuilder, FieldInfo _this, FieldInfo _item, MethodInfo removing)
        {
            MethodBuilder builder = typeBuilder.DefineMethod(
                $"Removing",
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName,
                typeof(void),
                Type.EmptyTypes);
            
            ILGenerator lout = builder.GetILGenerator();
            
            lout.Emit(OpCodes.Ldarg_0);
            lout.Emit(OpCodes.Ldfld, _this);
            lout.Emit(OpCodes.Ldarg_0);
            lout.Emit(OpCodes.Ldfld, _item);
            lout.Emit(OpCodes.Call, removing);
            
            lout.Emit(OpCodes.Ret);
        }
        
    }
}