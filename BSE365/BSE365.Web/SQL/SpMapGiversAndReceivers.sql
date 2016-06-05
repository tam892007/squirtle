CREATE PROCEDURE MapGiversAndReceivers
AS
BEGIN
	
	SET NOCOUNT ON;
	
	-------------------------------------
	-- check data
	DECLARE @giverNum INT
		,@receiverNum INT
		,@groupNum INT;

	SELECT @giverNum = count(*)
	FROM WaitingGivers;

	SELECT @receiverNum = count(*)
	FROM WaitingReceivers
	WHERE Amount = 3;

	IF (@giverNum / 3 < @receiverNum / 2)
		SET @groupNum = @giverNum / 3
	ELSE
		SET @groupNum = @receiverNum / 2;

	-- SELECT @giverNum AS Givers, @receiverNum AS Receivers, @groupNum AS Groups; -- view data
	-------------------------------------
	-- create group
	DECLARE @giverTemp TABLE (
		[Id] [int] NOT NULL
		,[AccountId] [nvarchar](128) NOT NULL
		,[Priority] [int] NOT NULL
		,[Created] [datetime] NOT NULL
		);
	DECLARE @receiverTemp TABLE (
		[Id] [int] NOT NULL
		,[AccountId] [nvarchar](128) NOT NULL
		,[Priority] [int] NOT NULL
		,[Created] [datetime] NOT NULL
		);
	DECLARE @groupTemp TABLE (
		[Giver1Id] [nvarchar](max) NULL
		,[Giver2Id] [nvarchar](max) NULL
		,[Giver3Id] [nvarchar](max) NULL
		,[Receiver1Id] [nvarchar](max) NULL
		,[Receiver2Id] [nvarchar](max) NULL
		);

	-- select data
	INSERT INTO @giverTemp
	SELECT Id
		,AccountId
		,Priority
		,Created
	FROM WaitingGivers
	ORDER BY Priority DESC
		,Created;

	INSERT INTO @receiverTemp
	SELECT Id
		,AccountId
		,Priority
		,Created
	FROM WaitingReceivers
	WHERE Amount = 3
	ORDER BY Priority DESC
		,Created;

	DECLARE @cnt INT = 0;

	WHILE @cnt < @groupNum
	BEGIN
		DECLARE @g1 INT = @cnt * 3 + 1
			,@g2 INT = @cnt * 3 + 2
			,@g3 INT = @cnt * 3 + 3
			,@r1 INT = @cnt * 2 + 1
			,@r2 INT = @cnt * 2 + 2
			,@giver1 [nvarchar](128)
			,@giver2 [nvarchar](128)
			,@giver3 [nvarchar](128)
			,@receiver1 [nvarchar](128)
			,@receiver2 [nvarchar](128);

		-- select givers ids
		WITH DataSource (
			AccountId
			,[rowID]
			)
		AS (
			SELECT AccountId
				,ROW_NUMBER() OVER (
					ORDER BY AccountId
					)
			FROM @giverTemp
			)
		SELECT @giver1 = IIF([rowID] = @g1, AccountId, @giver1)
			,@giver2 = IIF([rowID] = @g2, AccountId, @giver2)
			,@giver3 = IIF([rowID] = @g3, AccountId, @giver3)
		FROM DataSource;

		-- select receiver ids
		WITH DataSource (
			AccountId
			,[rowID]
			)
		AS (
			SELECT AccountId
				,ROW_NUMBER() OVER (
					ORDER BY id
					)
			FROM @receiverTemp
			)
		SELECT @receiver1 = IIF([rowID] = @r1, AccountId, @receiver1)
			,@receiver2 = IIF([rowID] = @r2, AccountId, @receiver2)
		FROM DataSource;

		-- insert to temp table
		INSERT INTO @groupTemp (
			[Giver1Id]
			,[Giver2Id]
			,[Giver3Id]
			,[Receiver1Id]
			,[Receiver2Id]
			)
		VALUES (
			@giver1
			,@giver2
			,@giver3
			,@receiver1
			,@receiver2
			);

		SET @cnt = @cnt + 1;
	END

	-- SELECT * FROM @groupTemp; -- view data
	-------------------------------------
	-- create money transaction
	DECLARE @giverId1 [nvarchar] (128)
		,@giverId2 [nvarchar] (128)
		,@giverId3 [nvarchar] (128)
		,@receiverId1 [nvarchar] (128)
		,@receiverId2 [nvarchar] (128);

	DECLARE Group_Cursor CURSOR LOCAL STATIC READ_ONLY FORWARD_ONLY
	FOR
	SELECT [Giver1Id]
		,[Giver2Id]
		,[Giver3Id]
		,[Receiver1Id]
		,[Receiver2Id]
	FROM @groupTemp;

	OPEN Group_Cursor

	FETCH NEXT
	FROM Group_Cursor
	INTO @giverId1
		,@giverId2
		,@giverId3
		,@receiverId1
		,@receiverId2;

	WHILE @@FETCH_STATUS = 0
	BEGIN
		-- SELECT @giverId1,@giverId2,@giverId3,@receiverId1,@receiverId2; -- view data
		-------------------------------------
		-- insert real data
		SET XACT_ABORT ON

		BEGIN TRANSACTION CreateMoneyTransaction;

		DECLARE @groupId INT;

		-- create money transfer group
		INSERT INTO MoneyTransferGroups (
			[Giver1Id]
			,[Giver2Id]
			,[Giver3Id]
			,[Receiver1Id]
			,[Receiver2Id]
			)
		VALUES (
			@giverId1
			,@giverId2
			,@giverId3
			,@receiverId1
			,@receiverId2
			)

		SET @groupId = SCOPE_IDENTITY();

		DECLARE @currentTime DATE = GETDATE();
		DECLARE @transaction_BeginState INT = 0;
		DECLARE @transaction_NotEnd BIT = 0;
		DECLARE @account_InGiveTransactionState INT = 11;
		DECLARE @account_InReceiveTransactionState INT = 12;

		-- create 6 transactions
		INSERT INTO [MoneyTransactions] (
			[GiverId]
			,[ReceiverId]
			,[Created]
			,[LastModified]
			,[State]
			,[MoneyTransferGroupId]
			,[IsEnd]
			)
		VALUES (
			-- #1
			@giverId1
			,@receiverId1
			,@currentTime
			,@currentTime
			,@transaction_BeginState
			,@groupId
			,@transaction_NotEnd
			)
			,(
			-- #2
			@giverId2
			,@receiverId1
			,@currentTime
			,@currentTime
			,@transaction_BeginState
			,@groupId
			,@transaction_NotEnd
			)
			,(
			-- #3
			@giverId3
			,@receiverId1
			,@currentTime
			,@currentTime
			,@transaction_BeginState
			,@groupId
			,@transaction_NotEnd
			)
			,(
			-- #4
			@giverId1
			,@receiverId2
			,@currentTime
			,@currentTime
			,@transaction_BeginState
			,@groupId
			,@transaction_NotEnd
			)
			,(
			-- #5
			@giverId2
			,@receiverId2
			,@currentTime
			,@currentTime
			,@transaction_BeginState
			,@groupId
			,@transaction_NotEnd
			)
			,(
			-- #6
			@giverId3
			,@receiverId2
			,@currentTime
			,@currentTime
			,@transaction_BeginState
			,@groupId
			,@transaction_NotEnd
			);

		-- update account state
		UPDATE Accounts
		SET STATE = @account_InGiveTransactionState
		WHERE UserName IN (
				@giverId1
				,@giverId2
				,@giverId3
				);

		UPDATE Accounts
		SET STATE = @account_InReceiveTransactionState
		WHERE UserName IN (
				@receiverId1
				,@receiverId2
				);

		-- delete giver queue
		DELETE
		FROM WaitingGivers
		WHERE AccountId IN (
				@giverId1
				,@giverId2
				,@giverId3
				);

		-- delete receiver queue 
		DELETE
		FROM WaitingReceivers
		WHERE AccountId IN (
				@receiverId1
				,@receiverId2
				);

		COMMIT TRANSACTION CreateMoneyTransaction;

		FETCH NEXT
		FROM Group_Cursor
		INTO @giverId1
			,@giverId2
			,@giverId3
			,@receiverId1
			,@receiverId2;
	END

	CLOSE Group_Cursor

	DEALLOCATE Group_Cursor
	-------------------------------------
END
GO


