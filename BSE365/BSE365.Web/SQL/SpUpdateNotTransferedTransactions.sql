CREATE PROCEDURE UpdateNotTransferedTransactions @Time DATETIME
AS
BEGIN
	SET NOCOUNT ON;

	-------------------------------------
	DECLARE @now DATETIME = GetDate();
	DECLARE @accountStateNotGive INT = 21
		,@accountStateInReceiveTransaction INT = 12
		,@accountStateDefault INT = 0
		,@transactionStateBegin INT = 0
		,@transactionStateConfirmed INT = 2
		,@transactionStateNotTransfer INT = 21
	DECLARE @transactionTemp TABLE (
		[Id] [int] NOT NULL
		,[GiverId] [nvarchar](128) NOT NULL
		,[ReceiverId] [nvarchar](128) NOT NULL
		,[Created] [datetime] NOT NULL
		,[LastModified] [datetime] NOT NULL
		,[State] [int] NOT NULL
		,[TransferedDate] [datetime] NULL
		,[MoneyTransferGroupId] [int] NULL
		);

	-------------------------------------
	-- select transactions
	INSERT INTO @transactionTemp
	SELECT Id
		,GiverId
		,ReceiverId
		,Created
		,LastModified
		,STATE
		,TransferedDate
		,MoneyTransferGroupId
	FROM MoneyTransactions
	WHERE IsEnd = 0
		AND STATE = @transactionStateBegin
		AND Created <= @Time
	ORDER BY Created;

	-- SELECT * FROM @transactionTemp
	-------------------------------------
	-- update money transactions
	DECLARE @tranId [int]
		,@tranGroupId [int];
	DECLARE @giverId [nvarchar] (128)
		,@receiverId [nvarchar] (128);

	DECLARE Transaction_Cursor CURSOR LOCAL STATIC READ_ONLY FORWARD_ONLY
	FOR
	SELECT [Id]
		,[GiverId]
		,[ReceiverId]
		,[MoneyTransferGroupId]
	FROM @transactionTemp;

	OPEN Transaction_Cursor

	FETCH NEXT
	FROM Transaction_Cursor
	INTO @tranId
		,@giverId
		,@receiverId
		,@tranGroupId;

	WHILE @@FETCH_STATUS = 0
	BEGIN
		-- SELECT @tranId ,@giverId ,@receiverId ,@tranGroupId;
		-------------------------------------
		-- update real data
		SET XACT_ABORT ON

		BEGIN TRANSACTION UpdateMoneyTransaction;

		-- update transaction
		UPDATE MoneyTransactions
		SET STATE = @transactionStateNotTransfer
			,LastModified = @now
			,IsEnd = 1
		WHERE Id = @tranId;

		-- update giver account
		UPDATE Accounts
		SET STATE = @accountStateNotGive
			,RelatedTransaction = ISNULL(CONVERT(NVARCHAR(10), RelatedTransaction), '') + CAST(@tranId AS NVARCHAR(128)) + ','
		WHERE UserName = @giverId;

		-- update receiver account
		DECLARE @unsuccessTranCount INT;

		SELECT @unsuccessTranCount = count(*)
		FROM MoneyTransactions
		WHERE MoneyTransferGroupId = @tranGroupId
			AND ReceiverId = @receiverId
			AND IsEnd = 0;

		IF (@unsuccessTranCount = 0)
		BEGIN
			UPDATE Accounts
			SET STATE = @accountStateDefault
			WHERE UserName = @receiverId
				AND STATE = @accountStateInReceiveTransaction;
		END

		COMMIT TRANSACTION UpdateMoneyTransaction;

		FETCH NEXT
		FROM Transaction_Cursor
		INTO @tranId
			,@giverId
			,@receiverId
			,@tranGroupId;
	END

	CLOSE Transaction_Cursor

	DEALLOCATE Transaction_Cursor
		-------------------------------------
END
GO


