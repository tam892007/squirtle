CREATE PROCEDURE UpdateNotConfirmedTransactions @Time DATETIME
AS
BEGIN
	SET NOCOUNT ON;

	-------------------------------------
	DECLARE @now DATETIME = GetDate();
	DECLARE @accountStateNotConfirm INT = 22
		,@accountStateInGiveTransaction INT = 11
		,@accountStateGave INT = 2
		,@transactionStateTransfered INT = 1
		,@transactionStateConfirmed INT = 2
		,@transactionStateNotConfirm INT = 22;
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
		AND STATE = @transactionStateTransfered
		AND TransferedDate <= @Time
	ORDER BY TransferedDate;

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
		SET STATE = @transactionStateNotConfirm
			,LastModified = @now
			,IsEnd = 1
		WHERE Id = @tranId;

		-- update receiver account
		UPDATE Accounts
		SET STATE = @accountStateNotConfirm
			,RelatedTransaction = ISNULL(CONVERT(NVARCHAR(10), RelatedTransaction), '') + CAST(@tranId AS NVARCHAR(128)) + ','
		WHERE UserName = @receiverId;

		-- update giver account
		DECLARE @unsuccessTranCount INT;

		SELECT @unsuccessTranCount = count(*)
		FROM MoneyTransactions
		WHERE MoneyTransferGroupId = @tranGroupId
			AND GiverId = @giverId
			AND IsEnd = 0;

		IF (@unsuccessTranCount = 0)
		BEGIN
			UPDATE Accounts
			SET STATE = @accountStateGave
			WHERE UserName = @giverId
				AND STATE = @accountStateInGiveTransaction;
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


