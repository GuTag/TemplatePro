create table Left_Top(
	ID INTEGER PRIMARY KEY AUTOINCREMENT,
	ML1Pre varchar(100),
	ML2Pre varchar(100),
	currentLevel varchar(100),
	loadAmount varchar(100),
	createTime varchar(100)
);

create table Left_Center(
	ID INTEGER PRIMARY KEY AUTOINCREMENT,
	ML1Pre varchar(100),
	ML2Pre varchar(100),
	currentLevel varchar(100),
	loadAmount varchar(100),
	createTime varchar(100)
);

create table Right_Top(
	ID INTEGER PRIMARY KEY AUTOINCREMENT,
	ML1Pre varchar(100),
	ML2Pre varchar(100),
	currentLevel varchar(100),
	loadAmount varchar(100),
	createTime varchar(100)
);

create table Right_Ceneter(
	ID INTEGER PRIMARY KEY AUTOINCREMENT,
	ML1Pre varchar(100),
	ML2Pre varchar(100),
	currentLevel varchar(100),
	loadAmount varchar(100),
	createTime varchar(100)
);

create table Left_Bottom(
	ID INTEGER PRIMARY KEY AUTOINCREMENT,
	deviceName varchar(100),
	onError varchar(100),
	contentMsg varchar(100),
	createTime varchar(100)
);

create table Right_Bottom(
	ID INTEGER PRIMARY KEY AUTOINCREMENT,
	deviceName varchar(100),
	colorCode varchar(100),
	currentLevel varchar(100),
	currentML1Pre varchar(100),
	currentML2Pre varchar(100),
	contentMsg varchar(100),
	createTime varchar(100)
);

create table Cenetr_Top(
	ID INTEGER PRIMARY KEY AUTOINCREMENT,
	deviceName varchar(100),
	status varchar(100),
	runMode varchar(100),
	colorCode varchar(100),
	createTime varchar(100)
);


create table Cenetr_Center(
	ID INTEGER PRIMARY KEY AUTOINCREMENT,
	Sys1FltNbr varchar(100),
	Sys2FltNbr varchar(100),
	Sys3FltNbr varchar(100),
	Sys4FltNbr varchar(100),
	totalNum varchar(100),
	createTime varchar(100)
);

create table Cenetr_Bottom(
	ID INTEGER PRIMARY KEY AUTOINCREMENT,
	Sys1Level varchar(100),
	Sys2Level varchar(100),
	Sys3Level varchar(100),
	Sys4Level varchar(100),
	createTime varchar(100)
);

