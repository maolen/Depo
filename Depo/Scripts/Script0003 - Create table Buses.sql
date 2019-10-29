USE DepoDb;

CREATE TABLE Buses (
	[Id] int  not null primary key identity,
	[VinNumber] nvarchar(MAX) null,
	[EngineerId] int null FOREIGN KEY REFERENCES Engineers(Id),
	[StatusId] int  null FOREIGN KEY REFERENCES Statuses(Id)
);