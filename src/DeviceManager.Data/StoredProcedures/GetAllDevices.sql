CREATE OR ALTER PROCEDURE GetAllDevices
    AS
BEGIN
    SET NOCOUNT ON;

SELECT
    d.Id,
    d.Name,
    d.Type,
    d.IsEnabled
FROM Devices d;
END
