using System;
using Spend_Management;

namespace Expenses_Reports.Formula
{
    internal static class TypeSelectorFactory
    {
        internal static ITypeSelector New(object columnValue)
        {
            if (columnValue is string)
            {
                return new StringTypeSelector();
            }

            if (columnValue is int)
            {
                return new IntTypeSelector();
            }

            if (columnValue is DateTime)
            {
                return new DateTypeSelector();
            }

            if (columnValue is double || columnValue is Currency || columnValue is decimal)
            {
                return new NumberTypeSelector();
            }

            if (columnValue is DBNull)
            {
                return new NullTypeSelector();
            }

            return new StringTypeSelector();
        }
    }
}