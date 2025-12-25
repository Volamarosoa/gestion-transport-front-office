CREATE DATABASE GestionTransport;
GO

USE GestionTransport;
GO

CREATE TABLE Employe (
                         Id INT PRIMARY KEY IDENTITY(1,1),
                         Nom NVARCHAR(100) NOT NULL,
                         Prenom NVARCHAR(100) NOT NULL,
                         AdresseDepart NVARCHAR(255) NOT NULL,
                         AdresseArrivee NVARCHAR(255) NOT NULL,
                         Telephone NVARCHAR(20),
                         DateCreation DATETIME DEFAULT GETDATE()
);
GO

INSERT INTO Employe (Nom, Prenom, AdresseDepart, AdresseArrivee, Telephone)
VALUES 
    ('Rakoto', 'Jean', '123 Rue Analakely, Antananarivo', 'Avenue de l''Indépendance, Antananarivo', '034 12 345 67'),
    ('Rabe', 'Marie', '456 Rue Ankorondrano, Antananarivo', 'Boulevard Ratsimilaho, Antananarivo', '033 98 765 43'),
    ('Randria', 'Paul', '789 Ambohijatovo, Antananarivo', 'Rue Rainibetsimisaraka, Antananarivo', '032 55 666 77');
GO