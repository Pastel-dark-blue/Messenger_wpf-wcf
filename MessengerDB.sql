-- ���� �� Chat �� ����������, ������� �
if not exists (
	select * from sys.databases
	where name = 'Chat')
begin
	create database Chat;
end;

use Chat;
-- ���� ������� �� ����������, ������� �

-- ������� � ������� � ������������
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

-- ������� � ������� � ������ ��������� ���������
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

-- ����
if not exists (
	select * from sys.objects
	where object_id = object_id('Chat'))
begin
	create table Chat(
		Id bigint primary key identity(1, 1),
		Name varchar(40),		-- �������� (��� ����� � 3-�� � ����� ���-��� �������������)
		Admin bigint foreign key references ChatUser(Id), 
	);
end;

-- id ����� � id �������������, ��������� � ���� �����
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