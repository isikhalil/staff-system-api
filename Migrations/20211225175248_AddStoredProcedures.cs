using Microsoft.EntityFrameworkCore.Migrations;

namespace Web.Migrations
{
    public partial class AddStoredProcedures : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			var sp = @"SET ANSI_NULLS ON
				GO
				SET QUOTED_IDENTIFIER ON
				GO

				CREATE PROCEDURE AddLog
					@Category NVARCHAR(63),
					@Message NVARCHAR(511)
				AS
				BEGIN
					SET NOCOUNT ON;

					INSERT INTO [dbo].[Logs]
						([CreatedOn]
						,[Category]
						,[Message])
					VALUES
						(GETDATE(),
						@Category,
						@Message)

				END
				GO

				-----------------------------------------

				SET ANSI_NULLS ON
				GO
				SET QUOTED_IDENTIFIER ON
				GO
				CREATE PROCEDURE GetStaffsByName
				AS
				BEGIN
					SET NOCOUNT ON;

					SELECT * FROM Staffs ORDER BY FirstName
				END

				-----------------------------------------

				SET ANSI_NULLS ON
				GO
				SET QUOTED_IDENTIFIER ON
				GO
				CREATE PROCEDURE CreateStaff
					@FirstName NVARCHAR(50),
					@LastName NVARCHAR(50),
					@BirthDate SMALLDATETIME,
					@Email NVARCHAR(127),
					@ProvinceId BIGINT,
					@TitleId BIGINT
				AS
				BEGIN
					SET NOCOUNT ON;

					INSERT INTO [dbo].[Staffs]
						([FirstName]
						,[LastName]
						,[BirthDate]
						,[Email]
						,[ProvinceId])
					 VALUES
						(@FirstName,
						@LastName,
						@BirthDate,
						@Email,
					  @ProvinceId)

					DECLARE @StaffId BIGINT = (SELECT SCOPE_IDENTITY())

					DECLARE @LogMessage NVARCHAR(511) = (SELECT CONCAT(@StaffId, ', ', @FirstName, ', ', @LastName, ', ', @BirthDate, ', ', @Email, ', ', @ProvinceId))

					EXEC AddLog 'Staff Insert', @LogMessage

					INSERT INTO [dbo].[StaffTitles]
						([StaffId]
						,[TitleId]
						,[StartDate])
					VALUES
						(@StaffId,
						@TitleId,
						GETDATE())

					SET @LogMessage = (SELECT CONCAT(@StaffId, ', ', @TitleId))
					EXEC AddLog 'Title Change', @LogMessage

					SELECT * FROM Staffs WHERE Id = @StaffId
				END
				GO

				---------------------------------------

				SET ANSI_NULLS ON
				GO
				SET QUOTED_IDENTIFIER ON
				GO
				CREATE PROCEDURE UpdateStaff
					@Id BIGINT,
					@FirstName NVARCHAR(50),
					@LastName NVARCHAR(50),
					@BirthDate SMALLDATETIME,
					@Email NVARCHAR(127),
					@ProvinceId BIGINT,
					@TitleId BIGINT
				AS
				BEGIN
					SET NOCOUNT ON;

					DECLARE @StaffsCount INT = (SELECT COUNT(*) FROM Staffs WHERE Id = @Id)

					IF @StaffsCount = 1
					BEGIN
						UPDATE Staffs 
						SET 
							FirstName = @FirstName,
							LastName = @LastName,
							BirthDate = @BirthDate,
							Email = @Email,
							ProvinceId = @ProvinceId
						WHERE Id = @Id

						DECLARE @LogMessage NVARCHAR(511) = (SELECT CONCAT(@FirstName, ', ', @LastName, ', ', @BirthDate, ', ', @Email, ', ', @ProvinceId))
						EXEC AddLog 'Staff Update', @LogMessage

						DECLARE @LastTitleId BIGINT = (SELECT TOP 1 TitleId FROM StaffTitles WHERE StaffId = @Id ORDER BY StartDate DESC)

						IF @LastTitleId <> @TitleId 
						BEGIN
							INSERT INTO [dbo].[StaffTitles]
								([StaffId]
								,[TitleId]
								,[StartDate])
							VALUES
								(@Id,
								@TitleId,
								GETDATE())

							SET @LogMessage = (SELECT CONCAT(@Id, ', ', @TitleId))
							EXEC AddLog 'Title Change', @LogMessage
						END
					END

					SELECT * FROM Staffs WHERE Id = @Id
				END
				GO

				------------------------------------

				SET ANSI_NULLS ON
				GO
				SET QUOTED_IDENTIFIER ON
				GO

				CREATE PROCEDURE FilterStaffs
					@ProvinceId BIGINT,
					@TitleId BIGINT,
					@MinAge INT
				AS
				BEGIN
					SET NOCOUNT ON;

					SELECT 
						S.Id StaffId,
						S.FirstName, S.LastName, S.BirthDate,
						DATEDIFF(HOUR, S.BirthDate, GETDATE()) / 8766 Age,
						P.[Name] Province,
						T.[Name] Title,
						ST.StartDate
					FROM StaffTitles ST
					INNER JOIN Staffs S ON S.Id = ST.StaffId
					INNER JOIN Titles T ON T.Id = ST.TitleId
					INNER JOIN Provinces P ON P.Id = S.ProvinceId
					WHERE 
						DATEDIFF(HOUR, S.BirthDate, GETDATE()) / 8766 >= @MinAge
						AND T.Id = @TitleId
						AND P.Id = @ProvinceId

				END
				GO";

			migrationBuilder.Sql(sp);
		}

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE UpdatePostNotifications");
        }
    }
}
