using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Helpdesk.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveTicketDraftSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "drafts",
                keyColumn: "Id",
                keyValue: new Guid("c1000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "drafts",
                keyColumn: "Id",
                keyValue: new Guid("c1000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "tickets",
                keyColumn: "Id",
                keyValue: new Guid("b1000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "tickets",
                keyColumn: "Id",
                keyValue: new Guid("b1000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "tickets",
                keyColumn: "Id",
                keyValue: new Guid("b1000000-0000-0000-0000-000000000002"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "tickets",
                columns: new[] { "Id", "Body", "Category", "CreatedAt", "MessageId", "SenderEmail", "SenderName", "SentAt", "State", "Subject", "Summary", "ThreadId", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("b1000000-0000-0000-0000-000000000001"), "Hi, I've tried resetting my password twice but I still get an \"invalid credentials\" error when I sign in. Can you help? — Priya", "AccountLogin", new DateTime(2026, 6, 1, 9, 0, 0, 0, DateTimeKind.Utc), "msg-0001", "priya.student@example.edu", "Priya Student", null, "NeedsReview", "Can't log in to my account", "Student cannot sign in; password reset did not resolve an invalid-credentials error.", "thread-0001", new DateTime(2026, 6, 1, 9, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("b1000000-0000-0000-0000-000000000002"), "I see two charges for the monthly plan on my card statement. Could you check and refund the duplicate if it's a mistake? Thanks, Sam", "Billing", new DateTime(2026, 6, 1, 9, 0, 0, 0, DateTimeKind.Utc), "msg-0002", "sam.buyer@example.com", "Sam Buyer", null, "NeedsReview", "Was I charged twice this month?", "Customer reports a possible duplicate monthly charge and requests a refund of the duplicate.", "thread-0002", new DateTime(2026, 6, 1, 9, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("b1000000-0000-0000-0000-000000000003"), "The course videos just show a black screen in Safari but work fine in Chrome. Is there a fix? — Lee", null, new DateTime(2026, 6, 1, 9, 0, 0, 0, DateTimeKind.Utc), "msg-0003", "lee.learner@example.edu", "Lee Learner", null, "NeedsReview", "Video lectures won't play on Safari", null, "thread-0003", new DateTime(2026, 6, 1, 9, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "drafts",
                columns: new[] { "Id", "Body", "CreatedAt", "IsAiGenerated", "TicketId", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("c1000000-0000-0000-0000-000000000001"), "Hi Priya,\n\nSorry for the trouble signing in. Let's get you back in:\n1) Use the \"Forgot password\" link and make sure you open the reset email within 30 minutes.\n2) Clear your browser cache or try an incognito window.\nIf it still fails, reply here and we'll reset it manually.\n\nBest,\nSupport", new DateTime(2026, 6, 1, 9, 0, 0, 0, DateTimeKind.Utc), true, new Guid("b1000000-0000-0000-0000-000000000001"), new DateTime(2026, 6, 1, 9, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("c1000000-0000-0000-0000-000000000002"), "Hi Sam,\n\nThanks for flagging this. I can see two charges for your monthly plan and have queued a refund for the duplicate — it should appear in 5–7 business days. You won't be charged twice going forward.\n\nBest,\nSupport", new DateTime(2026, 6, 1, 9, 0, 0, 0, DateTimeKind.Utc), true, new Guid("b1000000-0000-0000-0000-000000000002"), new DateTime(2026, 6, 1, 9, 0, 0, 0, DateTimeKind.Utc) }
                });
        }
    }
}
