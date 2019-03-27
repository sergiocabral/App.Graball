using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Graball.General.Data
{
    /// <summary>
    /// Extensão de métodos para enum
    /// </summary>
    public static class EnumExtension
    {
        /// <summary>
        /// Obter descrição amigável.
        /// </summary>
        /// <param name="genericEnum">Valor do enum</param>
        /// <returns>Descrição</returns>
        public static string GetDescription(this Enum genericEnum)
        {
            Type type = genericEnum.GetType();
            MemberInfo[] memberInfo = type.GetMember(genericEnum.ToString());
            if (memberInfo != null && memberInfo.Length > 0)
            {
                var attributes = memberInfo[0].GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
                if (attributes != null && attributes.Length > 0)
                {
                    return ((System.ComponentModel.DescriptionAttribute)attributes.GetValue(0)).Description;
                }
            }
            return genericEnum.ToString();
        }

    }
}
