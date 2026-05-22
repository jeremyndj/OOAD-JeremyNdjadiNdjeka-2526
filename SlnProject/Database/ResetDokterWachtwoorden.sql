-- Zet alle dokter-wachtwoorden op SHA-256 hash van 'test' (zelfde als PasswordHasher in CLDokterspraktijk).
-- Eenmalig uitvoeren in SSMS op DokterspraktijkDB als inloggen met seed-hashes niet lukt.
USE [DokterspraktijkDB];
GO

UPDATE [dbo].[Dokter]
SET [paswoord] = N'9f86d081884c7d659a2feaa0c55ad015a3bf4f1b2b0b822cd15d6c15b0f00a08';
GO
