CREATE PROCEDURE UpdateTransactions 
	@Time DATETIME
AS
BEGIN
	EXEC UpdateNotTransferedTransactions @Time GO;
	EXEC UpdateNotConfirmedTransactions @Time GO;
END
GO


