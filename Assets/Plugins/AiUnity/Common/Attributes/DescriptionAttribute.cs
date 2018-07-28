// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : AiDesigner
// Created          : 06-20-2016
// Modified         : 01-11-2018
// ***********************************************************************
#if AIUNITY_CODExxx

using System;
//using System.ComponentModel;
using System.Linq;

namespace AiUnity.Common.Attributes
{
    /// <summary>
    /// Associates a description associated with a property or field.
    /// </summary>
    /// <seealso cref="System.ComponentModel.DisplayNameAttribute" />
    //[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class DescriptionAttribute : Attribute
    {
        #region Properties
        public virtual string Description { get { return DisplayNameValue; } }
        protected string DisplayNameValue { get; set; }
        #endregion

        #region Constructors
        public DescriptionAttribute()
        {

        }

        public DescriptionAttribute(string description)
        {
            DisplayNameValue = description;
        }
        #endregion
    }
}

#endif