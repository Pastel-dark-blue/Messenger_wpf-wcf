-- если Ѕƒ Chat не существует, создаем еЄ
if not exists (
	select * from sys.databases
	where name = 'Chat')
begin
	create database Chat;
end;

use Chat;
-- если таблицы не существует, создаем еЄ

-- таблица с данными о пользователе
if not exists (
	select * from sys.objects
	where object_id = object_id('ChatUser'))
begin
	create table ChatUser(
		Id bigint primary key identity(1, 1),
		Login nvarchar(30) not null,
		Email nvarchar(254) not null,
		Password nvarchar(50) not null,
		Photo varchar(max),
		About nvarchar(2000),
		LastTimeOnline smalldatetime,
		IsActiveAccount bit not null,
	);
end;

-- таблица с данными о каждом отдельном сообщении
if not exists (
	select * from sys.objects
	where object_id = object_id('Message'))
begin
	create table Message(
		Id bigint primary key identity(1, 1),
		Content nvarchar(3000) not null,
		CreationDate smalldatetime not null,
		SenderUserId bigint foreign key references ChatUser(Id),
	);
end;

-- чаты
if not exists (
	select * from sys.objects
	where object_id = object_id('Chat'))
begin
	create table Chat(
		Id bigint primary key identity(1, 1),
		Name varchar(40),		-- название (дл€ чатов с 3-м€ и более кол-вом пользователей)
		Admin bigint foreign key references ChatUser(Id), 
	);
end;

-- id чатов и id пользователей, состо€щих в этих чатах
if not exists (
	select * from sys.objects
	where object_id = object_id('Party'))
begin
	create table Party(
		Id bigint primary key identity(1, 1),
		ChatUser bigint foreign key references ChatUser(Id), 
		Chat bigint foreign key references Chat(Id),
	);
end;