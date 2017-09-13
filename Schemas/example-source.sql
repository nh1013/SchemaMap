CREATE DATABASE Source;

USE Source;

CREATE TABLE Country
(Name VARCHAR(35) NOT NULL UNIQUE,
 Code VARCHAR(4),
 Capital VARCHAR(35),
 Area FLOAT,
 Population INT
 );

CREATE TABLE City
(Name VARCHAR(35),
 Code VARCHAR(4),
 Country VARCHAR(4),
 Population INT,
 Longitude FLOAT,
 Latitude FLOAT
 );

CREATE TABLE Mountain
(Name VARCHAR(35),
 Height FLOAT,
 Type VARCHAR(10),
 Longitude FLOAT,
 Latitude FLOAT
 );

CREATE TABLE Sea
(Name VARCHAR(35),
 Depth FLOAT,
 Longitude FLOAT,
 Latitude FLOAT
 );

CREATE TABLE River
(Name VARCHAR(35),
 Length FLOAT,
 Longitude FLOAT,
 Latitude FLOAT
 );
