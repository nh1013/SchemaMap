CREATE DATABASE Target;

USE Target;

CREATE TABLE Location
(Code VARCHAR(4),
 Name VARCHAR(35),
 Population INT,
 Area FLOAT,
 Longitude FLOAT,
 Latitude FLOAT,
 CONSTRAINT Lon CHECK ((Longitude >= -180) AND (Longitude <= 180)),
 CONSTRAINT Lat CHECK ((Latitude >= -90) AND (Latitude <= 90)));

CREATE TABLE Geography
(Name VARCHAR(35),
 Type VARCHAR(10),
 Longitude FLOAT,
 Latitude FLOAT,
 CONSTRAINT Lon CHECK ((Longitude >= -180) AND (Longitude <= 180)),
 CONSTRAINT Lat CHECK ((Latitude >= -90) AND (Latitude <= 90)));