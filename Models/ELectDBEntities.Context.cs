﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace E_Lect.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class ELectDBEntities : DbContext
    {
        public ELectDBEntities()
            : base("name=ELectDBEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Admin> Admins { get; set; }
        public virtual DbSet<Candidate> Candidates { get; set; }
        public virtual DbSet<MockStudent> MockStudents { get; set; }
        public virtual DbSet<Party> Parties { get; set; }
        public virtual DbSet<Student> Students { get; set; }
        public virtual DbSet<Vote> Votes { get; set; }
        public virtual DbSet<VoteTime> VoteTimes { get; set; }
        public virtual DbSet<PostImage> PostImages { get; set; }
        public virtual DbSet<SaveImage> SaveImages { get; set; }
    }
}