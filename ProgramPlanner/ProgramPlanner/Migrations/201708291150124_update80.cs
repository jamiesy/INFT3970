namespace ProgramPlanner.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update80 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Replacements", new[] { "ReplacementCourse_CourseID" });

        }

        public override void Down()
        {
        }
    }
}
