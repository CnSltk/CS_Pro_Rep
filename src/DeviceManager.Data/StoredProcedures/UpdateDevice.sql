CREATE OR ALTER PROCEDURE UpdateDevice
@Id NVARCHAR(50),
@Name NVARCHAR(100),
@IsEnabled BIT,
@Type NVARCHAR(50),
@BatteryPercentage INT = NULL,
@OperationSystem NVARCHAR(100) = NULL,
@IpAddress NVARCHAR(50) = NULL,
@NetworkName NVARCHAR(100) = NULL
AS
BEGIN
SET NOCOUNT ON;

UPDATE Devices
SET Name = @Name,
IsEnabled = @IsEnabled
WHERE Id = @Id;

IF @Type = 'SW'
BEGIN
UPDATE Smartwatch
SET BatteryPercentage = @BatteryPercentage
WHERE DeviceId = @Id;
END
ELSE IF @Type = 'PC'
BEGIN
UPDATE PersonalComputer
SET OperationSystem = @OperationSystem
WHERE DeviceId = @Id;
END
ELSE IF @Type = 'ED'
BEGIN
UPDATE Embedded
SET IpAddress = @IpAddress,
NetworkName = @NetworkName
WHERE DeviceId = @Id;
END
END