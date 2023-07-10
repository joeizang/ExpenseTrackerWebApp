﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

#pragma warning disable 219, 612, 618
#nullable enable

namespace ExpenseMVC.Models
{
    public partial class DataProtectionContextModel
    {
        partial void Initialize()
        {
            var dataProtectionKey = DataProtectionKeyEntityType.Create(this);

            DataProtectionKeyEntityType.CreateAnnotations(dataProtectionKey);

            AddAnnotation("ProductVersion", "7.0.8");
            AddAnnotation("Relational:MaxIdentifierLength", 64);
        }
    }
}
