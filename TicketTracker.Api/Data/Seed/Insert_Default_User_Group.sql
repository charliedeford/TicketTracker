USE TicketTracker; 
BEGIN TRANSACTION;
INSERT INTO [Groups] (Name, Description)
VALUES ('User', 'Users that may submit tickets'), ('Support', 'Users that will process and resolve tickets'), ('Admin', 'Users that will manage user accounts');
COMMIT TRANSACTION;