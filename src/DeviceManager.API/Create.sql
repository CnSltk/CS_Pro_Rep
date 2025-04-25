CREATE TABLE Devices (
    Id NVARCHAR(50) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Type CHAR(2) NOT NULL,
    BatteryPercentage INT NULL,
    OperatingSystem NVARCHAR(100) NULL,
    IPAddress NVARCHAR(50) NULL,
    NetworkName NVARCHAR(100) NULL,
    IsTurnedOn BIT DEFAULT 1      
);
-- Smartwatch
INSERT INTO Devices (Id, Name, Type, IsTurnedOn, BatteryPercentage)
VALUES
('SW-1', 'Apple Watch SE2', 'SW', 1, 27);

-- Personal Computers
INSERT INTO Devices (Id, Name, Type, IsTurnedOn, OperatingSystem)
VALUES
('P-1', 'LinuxPC', 'P', 0, 'Linux Mint'),
('P-2', 'ThinkPad T440', 'P', 0, NULL);

-- Embedded Devices
INSERT INTO Devices (Id, Name, Type, IPAddress, NetworkName)
VALUES
('ED-1', 'Pi3', 'ED', '192.168.1.44', 'MD Ltd.Wifi-1'),
('ED-2', 'Pi4', 'ED', '192.168.1.45', 'eduroam')

