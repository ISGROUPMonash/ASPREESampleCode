﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Aspree.Data
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class AspreeEntities : DbContext
    {
        public AspreeEntities()
            : base("name=AspreeEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<EntitySubType> EntitySubTypes { get; set; }
        public DbSet<EntityType> EntityTypes { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Variable> Variables { get; set; }
        public DbSet<VariableCategory> VariableCategories { get; set; }
        public DbSet<VariableRole> VariableRoles { get; set; }
        public DbSet<VariableType> VariableTypes { get; set; }
        public DbSet<ValidationRule> ValidationRules { get; set; }
        public DbSet<Tenant> Tenants { get; set; }
    }
}