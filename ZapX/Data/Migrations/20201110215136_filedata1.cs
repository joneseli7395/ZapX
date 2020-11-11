using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ZapX.Data.Migrations
{
    public partial class filedata1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte[]>(
                name: "FileData",
                table: "TicketAttachments",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(max)",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte[]>(
                name: "FileData",
                table: "TicketAttachments",
                type: "varbinary(max)",
                nullable: true,
                oldClrType: typeof(byte[]));
        }
    }
}
