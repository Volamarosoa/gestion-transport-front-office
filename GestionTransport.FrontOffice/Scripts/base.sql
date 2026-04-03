CREATE DATABASE GestionTransport;
GO

USE GestionTransport;
GO

CREATE TABLE Departement (
                             Id INT PRIMARY KEY IDENTITY(1,1),
                             Nom NVARCHAR(100) NOT NULL,
                             Description NVARCHAR(255),
                             Actif BIT NOT NULL DEFAULT 1,
                             DateInsertion DATETIME DEFAULT GETDATE(),
                             DateDesactivation DATETIME NULL
);
GO

CREATE TABLE Employe (
                         Id INT PRIMARY KEY IDENTITY(1,1),
                         Nom NVARCHAR(100) NOT NULL,
                         Prenom NVARCHAR(100) NOT NULL,
                         Matricule NVARCHAR(50),
                         Telephone NVARCHAR(15),
                         Email NVARCHAR(150) NULL,
                         IdDepartement INT FOREIGN KEY REFERENCES Departement(Id),
                         Actif BIT NOT NULL DEFAULT 1,
                         EstBeneficiaire BIT NOT NULL DEFAULT 0,
                         DateInsertion DATETIME DEFAULT GETDATE(),
                         DateDesactivation DATETIME NULL
);
GO

CREATE TABLE Role (
                      Id INT PRIMARY KEY IDENTITY(1,1),
                      Libelle NVARCHAR(50) NOT NULL  -- ADMIN, EMPLOYE
);
GO

CREATE TABLE Authentification (
                                  Id INT PRIMARY KEY IDENTITY(1,1),
                                  IdEmploye INT FOREIGN KEY REFERENCES Employe(Id),
                                  MotDePasse NVARCHAR(255) NOT NULL,  -- BCrypt hashé
                                  IdRole INT FOREIGN KEY REFERENCES Role(Id),
                                  ResetToken NVARCHAR(200) NULL,
                                  ResetTokenExpirationUtc DATETIME NULL,
                                  Actif BIT NOT NULL DEFAULT 1,
                                  DateCreation DATETIME DEFAULT GETDATE()
);
GO

-- Évite les doublons de matricule tout en permettant le NULL
CREATE UNIQUE INDEX IX_Employe_Matricule ON Employe(Matricule) WHERE Matricule IS NOT NULL;
GO

CREATE TABLE AdresseEmploye (
                                Id INT PRIMARY KEY IDENTITY(1,1),
                                IdEmploye INT FOREIGN KEY REFERENCES Employe(Id),
                                Adresse NVARCHAR(255),
                                Latitude DECIMAL(18,9),
                                Longitude DECIMAL(18,9),
                                EstPrincipale BIT DEFAULT 0,
                                Actif BIT NOT NULL DEFAULT 1,
                                DateInsertion DATETIME DEFAULT GETDATE()
);
GO

CREATE TABLE Site (
                      Id INT PRIMARY KEY IDENTITY(1,1),
                      Nom NVARCHAR(100), -- Siège, Usine, Agence, ...
                      Adresse NVARCHAR(255),
                      Latitude DECIMAL(18,9),
                      Longitude DECIMAL(18,9),
                      Actif BIT NOT NULL DEFAULT 1,
                      DateInsertion DATETIME DEFAULT GETDATE()
);
GO

CREATE TABLE Vehicule (
                          Id INT PRIMARY KEY IDENTITY(1,1),
                          Matricule NVARCHAR(50),
                          NombrePlaces INT,
                          Actif BIT NOT NULL DEFAULT 1,
                          DateInsertion DATETIME DEFAULT GETDATE(),
                          DateDesactivation DATETIME NULL
);
GO

CREATE TABLE TypeTransport (
                               Id INT PRIMARY KEY IDENTITY(1,1),
                               Libelle NVARCHAR(100), -- Aller, Retour
                               Actif BIT NOT NULL DEFAULT 1
);
GO

CREATE TABLE HeureTransport (
                                Id INT PRIMARY KEY IDENTITY(1,1),
                                Heure TIME,
                                Libelle NVARCHAR(100), -- Matin, Soir, Nuit
                                Actif BIT NOT NULL DEFAULT 1,
                                DateInsertion DATETIME DEFAULT GETDATE()
);
GO

CREATE TABLE DateTransport (
                               Id INT PRIMARY KEY IDENTITY(1,1),
                               DateJour DATE NOT NULL,
                               Actif BIT NOT NULL DEFAULT 1
);
GO

CREATE TABLE TypeAffectation (
                                 Id INT PRIMARY KEY IDENTITY(1,1),
                                 Libelle NVARCHAR(50), -- Automatique, Manuel
                                 Actif BIT NOT NULL DEFAULT 1
);
GO

CREATE TABLE Affectation (
                             Id INT PRIMARY KEY IDENTITY(1,1),
                             DateTransport DATE,
                             IdEmploye INT FOREIGN KEY REFERENCES Employe(Id),
                             IdAdresse INT FOREIGN KEY REFERENCES AdresseEmploye(Id),
                             IdTypeTransport INT FOREIGN KEY REFERENCES TypeTransport(Id),
                             IdSite INT FOREIGN KEY REFERENCES Site(Id),
                             IdVehicule INT FOREIGN KEY REFERENCES Vehicule(Id),
                             IdHeureTransport INT FOREIGN KEY REFERENCES HeureTransport(Id),

                             EstValidee BIT DEFAULT 0,
                             Commentaire NVARCHAR(255),

                             DateCreation DATETIME DEFAULT GETDATE(),
                             DateValidation DATETIME NULL,
                             IdType INT FOREIGN KEY REFERENCES TypeAffectation(Id),

                             EstArchive BIT DEFAULT 0  -- 0 = actif, 1 = archivé
);
GO

CREATE TABLE HistoriqueAffectation (
                                       IdHistorique INT PRIMARY KEY IDENTITY(1,1),
                                       IdAffectation INT FOREIGN KEY REFERENCES Affectation(Id),

    [Date] DATE,
                                       IdEmploye INT,
                                       IdAdresse INT,
                                       IdTypeTransport INT,
                                       IdSite INT,
                                       IdVehicule INT,
                                       IdHeureTransport INT,

                                       EstValidee BIT,
                                       Commentaire NVARCHAR(255),

                                       DateCreation DATETIME,
                                       DateValidation DATETIME,
                                       IdType INT,

                                       DateModification DATETIME DEFAULT GETDATE()
);
GO

CREATE INDEX IX_Affectation_Archive ON Affectation(EstArchive, DateTransport);
CREATE INDEX IX_Affectation_DateEmploye ON Affectation(DateTransport, IdEmploye) WHERE EstArchive = 0;

-- Archiver les affectations passées
UPDATE Affectation
SET EstArchive = 1
WHERE DateTransport < CAST(GETDATE() AS DATE)
  AND EstArchive = 0;
GO