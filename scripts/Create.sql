CREATE TABLE Device (
    Id NVARCHAR(50) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    IsEnabled BIT NOT NULL
);

CREATE TABLE Smartwatch (
    Id INT IDENTITY PRIMARY KEY,
    BatteryPercentage INT NOT NULL,
    DeviceId NVARCHAR(50),
    FOREIGN KEY (DeviceId) REFERENCES Device(Id)
);

CREATE TABLE PersonalComputer (
    Id INT IDENTITY PRIMARY KEY,
    OperationSystem NVARCHAR(100),
    DeviceId NVARCHAR(50),
    FOREIGN KEY (DeviceId) REFERENCES Device(Id)
);

CREATE TABLE Embedded (
    Id INT IDENTITY PRIMARY KEY,
    IpAddress NVARCHAR(50),
    NetworkName NVARCHAR(100),
    DeviceId NVARCHAR(50),
    FOREIGN KEY (DeviceId) REFERENCES Device(Id)
);