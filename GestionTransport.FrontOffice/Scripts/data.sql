USE GestionTransport;
GO

-- 1. Departement
INSERT INTO Departement (Nom, Description) VALUES
    ('Informatique', 'Département IT et développement'),
    ('Ressources Humaines', 'Gestion du personnel'),
    ('Finance', 'Comptabilité et finance'),
    ('Logistique', 'Gestion des transports et livraisons');
GO

-- 2. Employe
INSERT INTO Employe (Nom, Prenom, Matricule, Telephone, idDepartement) VALUES
    ('Rakoto', 'Jean', 'EMP001', '0341234567', 1),
    ('Rabe', 'Marie', 'EMP002', '0342345678', 2),
    ('Randria', 'Paul', 'EMP003', '0343456789', 3),
    ('Rasoa', 'Hanta', 'EMP004', '0344567890', 1),
    ('Raivo', 'Luc', 'EMP005', '0345678901', 4),
    ('Rafidy', 'Sarah', 'EMP006', '0346789012', 2),
    ('Ratsima', 'Pierre', 'EMP007', '0347890123', 3),
    ('Andriana', 'Noro', 'EMP008', '0348901234', 4);
GO

-- 3. AdresseEmploye
INSERT INTO AdresseEmploye (IdEmploye, Adresse, Latitude, Longitude, EstPrincipale) VALUES
    (1, 'Lot II A 34 Antananarivo', -18.910743, 47.536289, 1),
    (2, 'Lot III B 12 Ambohimanarina', -18.923456, 47.512345, 1),
    (3, 'Rue de France, Analakely', -18.916789, 47.523456, 1),
    (4, 'Lot V C 56 Itaosy', -18.934567, 47.498765, 1),
    (5, 'Quartier Ambanidia', -18.901234, 47.541234, 1),
    (6, 'Lot I D 78 Faravohitra', -18.908765, 47.528901, 1),
    (7, 'Avenue de l''Independance', -18.919876, 47.534567, 1),
    (8, 'Lot VII E 90 Ankadifotsy', -18.927654, 47.519876, 1);
GO

-- 4. SITE
INSERT INTO SITE (Nom, Adresse, Latitude, Longitude) VALUES
    ('Siège Social', 'Zone Industrielle Forello, Tanjombato', -18.963210, 47.521340),
    ('Usine Nord', 'Zone Industrielle Anosizato', -18.945678, 47.498765),
    ('Agence Centre', 'Analakely, Antananarivo', -18.916543, 47.523456);
GO

-- 5. Vehicule
INSERT INTO Vehicule (Matricule, NombrePlaces) VALUES
    ('IMM 1234 TA', 15),
    ('IMM 5678 TA', 20),
    ('IMM 9101 TA', 10),
    ('IMM 1121 TA', 25);
GO

-- 6. TypeTransport
INSERT INTO TypeTransport (Libelle) VALUES
    ('Aller'),
    ('Retour');
GO

-- 7. HeureTransport
INSERT INTO HeureTransport (Heure, Libelle) VALUES
    ('06:00:00', 'Matin'),
    ('14:00:00', 'Après-midi'),
    ('22:00:00', 'Nuit');
GO

-- 8. DateTransport
INSERT INTO DateTransport (DateJour) VALUES
    ('2026-03-22'),
    ('2026-03-23'),
    ('2026-03-24'),
    ('2026-03-25'),
    ('2026-03-26');
GO

-- 9. TypeAffectation
INSERT INTO TypeAffectation (Libelle) VALUES
    ('Automatique'),
    ('Manuel');
GO

-- 10. Affectation
INSERT INTO Affectation (IdDate, IdEmploye, IdAdresse, IdTypeTransport, IdSite, IdVehicule, IdHeureTransport, EstValidee, Commentaire, IdType) VALUES
    (1, 1, 1, 1, 1, 1, 1, 1, 'Trajet normal', 1),
    (1, 2, 2, 1, 1, 1, 1, 1, 'Trajet normal', 1),
    (1, 3, 3, 1, 2, 2, 1, 1, 'Trajet normal', 1),
    (1, 4, 4, 2, 1, 1, 2, 1, 'Retour soir', 1),
    (2, 5, 5, 1, 1, 3, 1, 0, 'En attente validation', 2),
    (2, 6, 6, 2, 2, 2, 2, 1, 'Retour validé', 1),
    (3, 7, 7, 1, 3, 4, 1, 1, 'Trajet matin', 2),
    (3, 8, 8, 2, 1, 1, 3, 0, 'Trajet nuit', 2),
    (4, 1, 1, 1, 1, 2, 1, 1, 'Trajet normal', 1),
    (5, 2, 2, 2, 2, 3, 2, NULL, 'Non encore validé', 2);
GO

-- 11. HistoriqueAffectation (simulating a modification on affectation 1)
INSERT INTO HistoriqueAffectation 
(IdAffectation, IdDate, IdEmploye, IdAdresse, IdTypeTransport, IdSite, IdVehicule, IdHeureTransport, EstValidee, Commentaire, DateCreation, DateValidation, IdType)
VALUES
    (1, 1, 1, 1, 1, 1, 2, 1, 0, 'Ancien véhicule avant modification', GETDATE(), NULL, 1),
    (3, 1, 3, 3, 1, 2, 1, 1, 0, 'Adresse modifiée', GETDATE(), NULL, 1);
GO
                
UPDATE Employe SET Email = 'jean.rakoto@company.mg' WHERE Id = 1;
UPDATE Employe SET Email = 'marie.rabe@company.mg' WHERE Id = 2;
UPDATE Employe SET Email = 'paul.randria@company.mg' WHERE Id = 3;
UPDATE Employe SET Email = 'hanta.rasoa@company.mg' WHERE Id = 4;
UPDATE Employe SET Email = 'luc.raivo@company.mg' WHERE Id = 5;
UPDATE Employe SET Email = 'sarah.rafidy@company.mg' WHERE Id = 6;
UPDATE Employe SET Email = 'pierre.ratsima@company.mg' WHERE Id = 7;
UPDATE Employe SET Email = 'noro.andriana@company.mg' WHERE Id = 8;
GO