using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplicationDemo.Data.Migrations
{
    public partial class addcolumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplyForJobs_Job_JobId",
                table: "ApplyForJobs");

            migrationBuilder.AlterColumn<int>(
                name: "JobId",
                table: "ApplyForJobs",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ApplyForJobs_Job_JobId",
                table: "ApplyForJobs",
                column: "JobId",
                principalTable: "Job",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplyForJobs_Job_JobId",
                table: "ApplyForJobs");

            migrationBuilder.AlterColumn<int>(
                name: "JobId",
                table: "ApplyForJobs",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplyForJobs_Job_JobId",
                table: "ApplyForJobs",
                column: "JobId",
                principalTable: "Job",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
