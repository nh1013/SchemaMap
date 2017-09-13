CREATE DATABASE Target;

USE Target;

CREATE TABLE Location
(Code VARCHAR(4),
 Name VARCHAR(35),
 Population INT,
 Area FLOAT,
 Longitude FLOAT,
 Latitude FLOAT);

CREATE TABLE Geography
(Name VARCHAR(35),
 Type VARCHAR(10),
 Longitude FLOAT,
 Latitude FLOAT);