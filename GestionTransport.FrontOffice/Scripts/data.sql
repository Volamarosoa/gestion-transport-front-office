USE GestionTransport;
GO

-- =============================================
-- ROLES
-- =============================================
INSERT INTO Role (Libelle) VALUES ('ADMIN');
INSERT INTO Role (Libelle) VALUES ('EMPLOYE');
GO

-- =============================================
-- DEPARTEMENTS
-- =============================================
INSERT INTO Departement (Nom, Description, Actif) VALUES ('Informatique', 'Direction des systèmes d''information', 1);
INSERT INTO Departement (Nom, Description, Actif) VALUES ('Ressources Humaines', 'Gestion du personnel', 1);
INSERT INTO Departement (Nom, Description, Actif) VALUES ('Finance', 'Direction financière et comptabilité', 1);
INSERT INTO Departement (Nom, Description, Actif) VALUES ('Logistique', 'Gestion des flux et transport', 1);
INSERT INTO Departement (Nom, Description, Actif) VALUES ('Production', 'Unité de production', 1);
GO

-- =============================================
-- EMPLOYES
-- =============================================
INSERT INTO Employe (Nom, Prenom, Matricule, Telephone, Email, IdDepartement, Actif, EstBeneficiaire)
VALUES ('Dupont', 'Jean', 'EMP001', '0341234567', 'jean.dupont@company.mg', 1, 1, 1);

INSERT INTO Employe (Nom, Prenom, Matricule, Telephone, Email, IdDepartement, Actif, EstBeneficiaire)
VALUES ('Martin', 'Marie', 'EMP002', '0342345678', 'marie.martin@company.mg', 2, 1, 1);

INSERT INTO Employe (Nom, Prenom, Matricule, Telephone, Email, IdDepartement, Actif, EstBeneficiaire)
VALUES ('Bernard', 'Pierre', 'EMP003', '0343456789', 'pierre.bernard@company.mg', 3, 1, 1);

INSERT INTO Employe (Nom, Prenom, Matricule, Telephone, Email, IdDepartement, Actif, EstBeneficiaire)
VALUES ('Rakoto', 'Hery', 'EMP004', '0344567890', 'hery.rakoto@company.mg', 4, 1, 1);

INSERT INTO Employe (Nom, Prenom, Matricule, Telephone, Email, IdDepartement, Actif, EstBeneficiaire)
VALUES ('Rabe', 'Soa', 'EMP005', '0345678901', 'soa.rabe@company.mg', 5, 1, 1);

INSERT INTO Employe (Nom, Prenom, Matricule, Telephone, Email, IdDepartement, Actif, EstBeneficiaire)
VALUES ('Andria', 'Aina', 'EMP006', '0346789012', 'aina.andria@company.mg', 1, 1, 1);

INSERT INTO Employe (Nom, Prenom, Matricule, Telephone, Email, IdDepartement, Actif, EstBeneficiaire)
VALUES ('Razafy', 'Tojo', 'EMP007', '0347890123', 'tojo.razafy@company.mg', 2, 1, 1);

INSERT INTO Employe (Nom, Prenom, Matricule, Telephone, Email, IdDepartement, Actif, EstBeneficiaire)
VALUES ('Rasolofo', 'Nivo', 'EMP008', '0348901234', 'nivo.rasolofo@company.mg', 3, 1, 1);
GO

-- =============================================
-- AUTHENTIFICATION (password = "password123" BCrypt hashé)
-- =============================================
-- Hash BCrypt de "password123" (cost=11)
INSERT INTO Authentification (IdEmploye, MotDePasse, IdRole, Actif)
VALUES (1, '$2a$11$3u66GfVRa561jQuDr6chYuYJ4jD3HX/pLLqFwNA.ShzA6ExuFhfzK', 2, 1); -- EMP001

INSERT INTO Authentification (IdEmploye, MotDePasse, IdRole, Actif)
VALUES (2, '$2a$11$3u66GfVRa561jQuDr6chYuYJ4jD3HX/pLLqFwNA.ShzA6ExuFhfzK', 2, 1); -- EMP002

INSERT INTO Authentification (IdEmploye, MotDePasse, IdRole, Actif)
VALUES (3, '$2a$11$3u66GfVRa561jQuDr6chYuYJ4jD3HX/pLLqFwNA.ShzA6ExuFhfzK', 2, 1); -- EMP003

INSERT INTO Authentification (IdEmploye, MotDePasse, IdRole, Actif)
VALUES (4, '$2a$11$3u66GfVRa561jQuDr6chYuYJ4jD3HX/pLLqFwNA.ShzA6ExuFhfzK', 2, 1); -- EMP004
GO

-- =============================================
-- ADRESSES EMPLOYES
-- =============================================
INSERT INTO AdresseEmploye (IdEmploye, Adresse, Latitude, Longitude, EstPrincipale, Actif)
VALUES (1, 'Lot II A 25, Antananarivo 101', -18.910657, 47.536659, 1, 1);

INSERT INTO AdresseEmploye (IdEmploye, Adresse, Latitude, Longitude, EstPrincipale, Actif)
VALUES (2, 'Rue Ratsimilaho, Faravohitra', -18.898765, 47.528432, 1, 1);

INSERT INTO AdresseEmploye (IdEmploye, Adresse, Latitude, Longitude, EstPrincipale, Actif)
VALUES (3, 'Cité des 67 Ha, Antananarivo', -18.933412, 47.521876, 1, 1);

INSERT INTO AdresseEmploye (IdEmploye, Adresse, Latitude, Longitude, EstPrincipale, Actif)
VALUES (4, 'Ankadifotsy, Antananarivo', -18.921543, 47.543210, 1, 1);

INSERT INTO AdresseEmploye (IdEmploye, Adresse, Latitude, Longitude, EstPrincipale, Actif)
VALUES (5, 'Ambohimanarina, Antananarivo', -18.895432, 47.501234, 1, 1);

INSERT INTO AdresseEmploye (IdEmploye, Adresse, Latitude, Longitude, EstPrincipale, Actif)
VALUES (6, 'Ankorondrano, Antananarivo', -18.906789, 47.531234, 1, 1);

INSERT INTO AdresseEmploye (IdEmploye, Adresse, Latitude, Longitude, EstPrincipale, Actif)
VALUES (7, 'Ivandry, Antananarivo', -18.883456, 47.547890, 1, 1);

INSERT INTO AdresseEmploye (IdEmploye, Adresse, Latitude, Longitude, EstPrincipale, Actif)
VALUES (8, 'Ambatobe, Antananarivo', -18.875432, 47.556789, 1, 1);

-- Adresse secondaire pour EMP001
INSERT INTO AdresseEmploye (IdEmploye, Adresse, Latitude, Longitude, EstPrincipale, Actif)
VALUES (1, 'Andoharanofotsy, Antananarivo', -18.975432, 47.521234, 0, 1);
GO

-- =============================================
-- SITES
-- =============================================
INSERT INTO Site (Nom, Adresse, Latitude, Longitude, Actif)
VALUES ('Siège Social', 'Ankorondrano, Antananarivo 101', -18.906543, 47.531876, 1);

INSERT INTO Site (Nom, Adresse, Latitude, Longitude, Actif)
VALUES ('Usine Taomasina', 'Zone Industrielle, Toamasina', -18.157890, 49.401234, 1);

INSERT INTO Site (Nom, Adresse, Latitude, Longitude, Actif)
VALUES ('Agence Analakely', 'Avenue de l''Indépendance, Analakely', -18.916789, 47.535432, 1);

INSERT INTO Site (Nom, Adresse, Latitude, Longitude, Actif)
VALUES ('Entrepôt Antsirabe', 'Zone Franche, Antsirabe', -19.865432, 47.031234, 1);
GO

-- =============================================
-- VEHICULES
-- =============================================
INSERT INTO Vehicule (Matricule, NombrePlaces, Actif)
VALUES ('IMM-0012-T', 15, 1);

INSERT INTO Vehicule (Matricule, NombrePlaces, Actif)
VALUES ('IMM-0034-T', 20, 1);

INSERT INTO Vehicule (Matricule, NombrePlaces, Actif)
VALUES ('IMM-0056-T', 10, 1);

INSERT INTO Vehicule (Matricule, NombrePlaces, Actif)
VALUES ('IMM-0078-T', 30, 1);

INSERT INTO Vehicule (Matricule, NombrePlaces, Actif)
VALUES ('IMM-0090-T', 8, 1);
GO

-- =============================================
-- TYPE TRANSPORT
-- =============================================
INSERT INTO TypeTransport (Libelle, Actif) VALUES ('Aller', 1);
INSERT INTO TypeTransport (Libelle, Actif) VALUES ('Retour', 1);
GO

-- =============================================
-- HEURE TRANSPORT
-- =============================================
INSERT INTO HeureTransport (Heure, Libelle, Actif) VALUES ('06:30', 'Très tôt matin', 1);
INSERT INTO HeureTransport (Heure, Libelle, Actif) VALUES ('07:30', 'Matin', 1);
INSERT INTO HeureTransport (Heure, Libelle, Actif) VALUES ('08:00', 'Matin tardif', 1);
INSERT INTO HeureTransport (Heure, Libelle, Actif) VALUES ('12:00', 'Midi', 1);
INSERT INTO HeureTransport (Heure, Libelle, Actif) VALUES ('17:00', 'Soir', 1);
INSERT INTO HeureTransport (Heure, Libelle, Actif) VALUES ('18:00', 'Soir tardif', 1);
INSERT INTO HeureTransport (Heure, Libelle, Actif) VALUES ('19:00', 'Nuit', 1);
GO

-- =============================================
-- TYPE AFFECTATION
-- =============================================
INSERT INTO TypeAffectation (Libelle, Actif) VALUES ('Automatique', 1);
INSERT INTO TypeAffectation (Libelle, Actif) VALUES ('Manuel', 1);
GO

-- =============================================
-- AFFECTATIONS (aujourd'hui + quelques historiques)
-- =============================================

-- Aujourd'hui validées (visibles sur la page Bienvenue)
INSERT INTO Affectation (DateTransport, IdEmploye, IdAdresse, IdTypeTransport, IdSite, IdVehicule, IdHeureTransport, EstValidee, Commentaire, IdType, EstArchive)
VALUES (CAST(GETDATE() AS DATE), 1, 1, 1, 1, 1, 2, 1, NULL, 1, 0); -- EMP001 Aller Matin Véhicule 1

INSERT INTO Affectation (DateTransport, IdEmploye, IdAdresse, IdTypeTransport, IdSite, IdVehicule, IdHeureTransport, EstValidee, Commentaire, IdType, EstArchive)
VALUES (CAST(GETDATE() AS DATE), 2, 2, 1, 1, 1, 2, 1, NULL, 1, 0); -- EMP002 Aller Matin Véhicule 1

INSERT INTO Affectation (DateTransport, IdEmploye, IdAdresse, IdTypeTransport, IdSite, IdVehicule, IdHeureTransport, EstValidee, Commentaire, IdType, EstArchive)
VALUES (CAST(GETDATE() AS DATE), 3, 3, 1, 3, 2, 3, 1, NULL, 1, 0); -- EMP003 Aller Matin Véhicule 2

INSERT INTO Affectation (DateTransport, IdEmploye, IdAdresse, IdTypeTransport, IdSite, IdVehicule, IdHeureTransport, EstValidee, Commentaire, IdType, EstArchive)
VALUES (CAST(GETDATE() AS DATE), 4, 4, 2, 1, 2, 5, 1, NULL, 1, 0); -- EMP004 Retour Soir Véhicule 2

INSERT INTO Affectation (DateTransport, IdEmploye, IdAdresse, IdTypeTransport, IdSite, IdVehicule, IdHeureTransport, EstValidee, Commentaire, IdType, EstArchive)
VALUES (CAST(GETDATE() AS DATE), 5, 5, 2, 3, 3, 6, 1, NULL, 1, 0); -- EMP005 Retour Soir Véhicule 3

-- Aujourd'hui en attente (pas encore validées)
INSERT INTO Affectation (DateTransport, IdEmploye, IdAdresse, IdTypeTransport, IdSite, IdVehicule, IdHeureTransport, EstValidee, Commentaire, IdType, EstArchive)
VALUES (CAST(GETDATE() AS DATE), 1, 1, 2, 1, NULL, 5, NULL, 'Demande retour soir', 1, 0); -- EMP001 en attente

INSERT INTO Affectation (DateTransport, IdEmploye, IdAdresse, IdTypeTransport, IdSite, IdVehicule, IdHeureTransport, EstValidee, Commentaire, IdType, EstArchive)
VALUES (CAST(GETDATE() AS DATE), 6, 6, 1, 1, NULL, 2, NULL, NULL, 1, 0); -- EMP006 en attente

-- Hier (archivées)
INSERT INTO Affectation (DateTransport, IdEmploye, IdAdresse, IdTypeTransport, IdSite, IdVehicule, IdHeureTransport, EstValidee, Commentaire, IdType, EstArchive)
VALUES (CAST(DATEADD(DAY, -1, GETDATE()) AS DATE), 1, 1, 1, 1, 1, 2, 1, NULL, 1, 1);

INSERT INTO Affectation (DateTransport, IdEmploye, IdAdresse, IdTypeTransport, IdSite, IdVehicule, IdHeureTransport, EstValidee, Commentaire, IdType, EstArchive)
VALUES (CAST(DATEADD(DAY, -1, GETDATE()) AS DATE), 2, 2, 1, 1, 1, 2, 1, NULL, 1, 1);

INSERT INTO Affectation (DateTransport, IdEmploye, IdAdresse, IdTypeTransport, IdSite, IdVehicule, IdHeureTransport, EstValidee, Commentaire, IdType, EstArchive)
VALUES (CAST(DATEADD(DAY, -1, GETDATE()) AS DATE), 3, 3, 2, 3, 2, 5, 0, 'Refusée', 1, 1);

-- Il y a 3 jours
INSERT INTO Affectation (DateTransport, IdEmploye, IdAdresse, IdTypeTransport, IdSite, IdVehicule, IdHeureTransport, EstValidee, Commentaire, IdType, EstArchive)
VALUES (CAST(DATEADD(DAY, -3, GETDATE()) AS DATE), 1, 1, 1, 1, 1, 2, 1, NULL, 1, 1);

INSERT INTO Affectation (DateTransport, IdEmploye, IdAdresse, IdTypeTransport, IdSite, IdVehicule, IdHeureTransport, EstValidee, Commentaire, IdType, EstArchive)
VALUES (CAST(DATEADD(DAY, -3, GETDATE()) AS DATE), 4, 4, 2, 1, 3, 6, 1, NULL, 1, 1);
GO

-- =============================================
-- HISTORIQUE AFFECTATION
-- =============================================
INSERT INTO HistoriqueAffectation (IdAffectation, [Date], IdEmploye, IdAdresse, IdTypeTransport, IdSite, IdVehicule, IdHeureTransport, EstValidee, Commentaire, DateCreation, DateValidation, IdType)
VALUES (8, CAST(DATEADD(DAY, -1, GETDATE()) AS DATE), 1, 1, 1, 1, 1, 2, 1, NULL, DATEADD(DAY,-1,GETDATE()), DATEADD(DAY,-1,GETDATE()), 1);

INSERT INTO HistoriqueAffectation (IdAffectation, [Date], IdEmploye, IdAdresse, IdTypeTransport, IdSite, IdVehicule, IdHeureTransport, EstValidee, Commentaire, DateCreation, DateValidation, IdType)
VALUES (11, CAST(DATEADD(DAY, -3, GETDATE()) AS DATE), 1, 1, 1, 1, 1, 2, 1, NULL, DATEADD(DAY,-3,GETDATE()), DATEADD(DAY,-3,GETDATE()), 1);
GO

-- =============================================
-- VERIFICATION
-- =============================================
SELECT 'Departements' AS Table_, COUNT(*) AS Total FROM Departement
UNION ALL SELECT 'Employes', COUNT(*) FROM Employe
UNION ALL SELECT 'Authentifications', COUNT(*) FROM Authentification
UNION ALL SELECT 'Adresses', COUNT(*) FROM AdresseEmploye
UNION ALL SELECT 'Sites', COUNT(*) FROM Site
UNION ALL SELECT 'Vehicules', COUNT(*) FROM Vehicule
UNION ALL SELECT 'TypeTransport', COUNT(*) FROM TypeTransport
UNION ALL SELECT 'HeureTransport', COUNT(*) FROM HeureTransport
UNION ALL SELECT 'TypeAffectation', COUNT(*) FROM TypeAffectation
UNION ALL SELECT 'Affectations', COUNT(*) FROM Affectation
UNION ALL SELECT 'Historique', COUNT(*) FROM HistoriqueAffectation;
GO