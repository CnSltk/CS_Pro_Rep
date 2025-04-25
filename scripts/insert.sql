INSERT INTO Device (Id, Name, IsEnabled) VALUES
('SW-1', 'Apple Watch SE2', 1),
('P-1', 'LinuxPC', 0),
('P-2', 'ThinkPad T440', 0),
('ED-1', 'Pi3', 1),
('ED-2', 'Pi4', 1);
INSERT INTO Smartwatch (BatteryPercentage, DeviceId) VALUES
(27, 'SW-1');
INSERT INTO PersonalComputer (OperationSystem, DeviceId) VALUES
('Linux Mint', 'P-1'),
(NULL, 'P-2');
INSERT INTO Embedded (IpAddress, NetworkName, DeviceId) VALUES
('192.168.1.44', 'MD Ltd.Wifi-1', 'ED-1'),
('192.168.1.45', 'eduroam', 'ED-2');