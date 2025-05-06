CREATE OR ALTER PROCEDURE GetDeviceById
    @Id NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        d.Id,
        d.Name,
        d.Type,
        d.IsEnabled
    FROM Devices d
    WHERE d.Id = @Id;
END
