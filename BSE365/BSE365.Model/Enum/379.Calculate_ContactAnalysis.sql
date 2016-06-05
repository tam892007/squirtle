USE [SCADev_MSCRM_Extensions_Online]
GO

/****** Object:  StoredProcedure [dbo].[Calculate_ContactAnalysis]    Script Date: 6/2/2016 10:23:27 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE procedure  [dbo].[Calculate_ContactAnalysis]
as
begin
	--------------------------------------------------------------------------------
	--Local variables
	--------------------------------------------------------------------------------
	--
	declare	@batch_type_income			int,
			@batch_type_gab			int,
			@processing_wait_time		varchar(8),
			@processing_retry_count		int,
			@today				datetime,
			@single_giving_lapsed_period	int,
			@regular_giving_lapsed_period	int,
			@regular_giving_delinquent_period int,
			@regular_giving_noshow_period	int,
			@regular_giving_cancelled_period int
		

	--------------------------------------------------------------------------------
	--Initialisation
	--------------------------------------------------------------------------------
	--
	set	@batch_type_income		= 814370000
	set	@batch_type_gab			= 814370001

	set	@processing_wait_time		= '00:00:10'
	set	@processing_retry_count		= 5
	set	@today				= cast(floor(cast(getdate() as float)) as datetime)

	set @single_giving_lapsed_period = (select cib_timeperioddays 
						from Syn_cib_donorstatusconfiguration 
						where cib_type = 912800000 and cib_endstatus = 912800002 --Single Gift Lapsed
					)

	set @regular_giving_lapsed_period = (select cib_timeperioddays 
						from Syn_cib_donorstatusconfiguration 
						where cib_type = 912800001 and cib_endstatus = 912800002 --Regular Gift Lapsed
					)

	set @regular_giving_delinquent_period = (select cib_timeperioddays 
							from Syn_cib_donorstatusconfiguration 
							where cib_type = 912800001 and cib_endstatus = 912800001 --Regular Gift Delinquent
						)

	set @regular_giving_noshow_period = (select cib_timeperioddays 
							from Syn_cib_donorstatusconfiguration 
							where cib_type = 912800001 and cib_endstatus = 912800000 --Regular Gift No Show
						)

	set @regular_giving_cancelled_period = (select cib_timeperioddays 
							from Syn_cib_donorstatusconfiguration 
							where cib_type = 912800001 and cib_endstatus = 912800003 --Regular Gift Cancelled
						)

	--------------------------------------------------------------------------------
	--Create temp table
	--------------------------------------------------------------------------------
	--
	create table [#ContactAnalysis]	
		(
		[id]							[uniqueidentifier]		NOT NULL,
		[cibernfp_totalincome]					[money]				NULL,
		[cibernfp_totalincomecount]				[int]				NULL,
		[cibernfp_totalvoluntaryincomeexcgiftaid]		[money]				NULL,
		[cibernfp_totalvoluntaryincomeexcgiftaidcount]		[int]				NULL,
		[cibernfp_totalvoluntaryincomeincgiftaid]		[money]				NULL,
		[cibernfp_totalvoluntaryincomeincgiftaidcount]		[int]				NULL,
		[cibernfp_datelastincome]				[datetime]			NULL,
		[cibernfp_age]						[int]				NULL,
		[cibernfp_volunteeringstatus]				[int]				NULL,
		[cibernli_flagshipmembershipstatus]			[int]				NULL,
		[cibernfp_totalincomeincgiftaid]			[money]				NULL,
		[cibernfp_MajorDonorStatus]				[int]				NULL,
		[cibernfp_donationacknowledgements]			[bit]				NULL,
		[cibernfp_directdebitmailingsasks]			[bit]				NULL,
		[cibernfp_giftaidmailings]				[bit]				NULL,
		[cibernfp_totalsinglegiftscount]		[int]				NULL,
		[cibernfp_totalregulargiftscount]			[int]				NULL,
		[cib_singlegivingstatus]				[int]				NULL,
		[cib_nextbirthdaydate]					[datetime]				NULL,
		[cib_nextpaymentanniversarydate]		[datetime]				NULL,
		[cibernfp_totalsinglegiftsexcgiftaid]	[money]					NULL,
		[cibernfp_totalregulargiftsexcgiftaid]	[money]					NULL,
		[cib_totalannualcumulativegift]			[money]					NULL,
		[cibernfp_datelastregulargift]			[datetime]				NULL,
		[cibernfp_datelastsinglegift]			[datetime]				NULL,

		[old_cibernfp_totalincome]					[money]				NULL,
		[old_cibernfp_totalincomecount]					[int]				NULL,
		[old_cibernfp_totalvoluntaryincomeexcgiftaid]			[money]				NULL,
		[old_cibernfp_totalvoluntaryincomeexcgiftaidcount]		[int]				NULL,
		[old_cibernfp_totalvoluntaryincomeincgiftaid]			[money]				NULL,
		[old_cibernfp_totalvoluntaryincomeincgiftaidcount]		[int]				NULL,
		[old_cibernfp_datelastincome]					[datetime]			NULL,
		[old_cibernfp_age]						[int]				NULL,
		[old_cibernfp_volunteeringstatus]				[int]				NULL,
		[old_cibernfp_totalincomeincgiftaid]				[money]				NULL,
		[old_cibernfp_MajorDonorStatus]					[int]				NULL,
		[old_cibernfp_donationacknowledgements]				[bit]				NULL,
		[old_cibernfp_directdebitmailingsasks]				[bit]				NULL,
		[old_cibernfp_giftaidmailings]					[bit]				NULL,
		[old_cibernfp_totalsinglegiftscount]		[int]				NULL,
		[old_cibernfp_totalregulargiftscount]			[int]				NULL,
		[old_cib_singlegivingstatus]				[int]				NULL,		
		[old_cib_nextbirthdaydate]					[datetime]				NULL,
		[old_cib_nextpaymentanniversarydate]		[datetime]				NULL,
		[old_cibernfp_totalsinglegiftsexcgiftaid]	[money]					NULL,
		[old_cibernfp_totalregulargiftsexcgiftaid]	[money]					NULL,
		[old_cib_totalannualcumulativegift]			[money]					NULL,
		[old_cibernfp_datelastregulargift]			[datetime]				NULL,
		[old_cibernfp_datelastsinglegift]			[datetime]				NULL,
		[checksum]							[bigint]			null,
		[old_checksum]							[bigint]			null
		)


	--------------------------------------------------------------------------------
	--Perform initial check to see if there is data remaining in the analysis
	--table. If there is, then wait and try again 
	--------------------------------------------------------------------------------
	--
	while	(
		select	COUNT(*) 
		from	dbo.ContactAnalysis (nolock)
		) > 0
	and	@processing_retry_count > 0
	begin
		waitfor delay @processing_wait_time
		
		set @processing_retry_count = @processing_retry_count - 1
		
	end


	--------------------------------------------------------------------------------
	--Having waited the required time, make a final check & then proceed if no 
	--rows are left in the table.
	--------------------------------------------------------------------------------
	--
	if	(
		select	COUNT(*) 
		from	dbo.ContactAnalysis (nolock)
		) = 0
	begin
		insert 
		into	[#ContactAnalysis] 
			(
			[id],
			[cibernfp_totalincome],
			[cibernfp_totalincomecount],
			[cibernfp_totalvoluntaryincomeexcgiftaid],
			[cibernfp_totalvoluntaryincomeexcgiftaidcount],
			[cibernfp_totalvoluntaryincomeincgiftaid],
			[cibernfp_totalvoluntaryincomeincgiftaidcount],
			[cibernfp_datelastincome],
			[cibernfp_age],
			[cibernfp_volunteeringstatus],
			[cibernfp_totalincomeincgiftaid],
			[cibernfp_MajorDonorStatus],
			[cibernfp_donationacknowledgements],
			[cibernfp_directdebitmailingsasks],
			[cibernfp_giftaidmailings],
			[cibernfp_totalsinglegiftscount],
			[cibernfp_totalregulargiftscount],
			[cib_singlegivingstatus],
			[cib_nextbirthdaydate],
			[cib_nextpaymentanniversarydate],
			[cibernfp_totalsinglegiftsexcgiftaid],
			[cibernfp_totalregulargiftsexcgiftaid],
			[cib_totalannualcumulativegift],
			[cibernfp_datelastregulargift],
			[cibernfp_datelastsinglegift]
			)
				
		select		d.cibernfp_contactid									as [id],
				CAST(SUM(d.total_income) as money)						as [cibernfp_totalincome],
				SUM(d.total_income_count)							as [cibernfp_totalincomecount],
				SUM(d.total_single_gift_income + d.total_regular_gift_income)			as [cibernfp_totalvoluntaryincomeexgiftaid],
				SUM(d.total_regular_gift_count	+ d.total_single_gift_count)			as [cibernfp_totalvoluntaryincomeexgiftaidcount],
				SUM(d.total_single_gift_income_inc_tax + d.total_regular_gift_income_inc_tax)	as [cibernfp_totalvoluntaryincomeincgiftaid],
				SUM(d.total_regular_gift_count	+ d.total_single_gift_count)			as [cibernfp_totalvoluntaryincomeincgiftaidcount],
				MAX(d.last_income_date)								as [cibernfp_datelastincome],
				MAX(d.age)									as [cibernfp_age],
				SUM(d.volunteeringstatus)							as [cibernfp_volunteeringstatus],
				SUM(d.total_single_gift_income_inc_tax + d.total_regular_gift_income_inc_tax)	as [cibernfp_totalincomeincgiftaid],
				null										as [cibernfp_MajorDonorStatus],
				sum(d.donationacknowledgements)							as donationacknowledgements,
				sum(d.directdebitmailingsasks)							as directdebitmailingsasks,
				sum(d.giftaidmailings)								as giftaidmailings,
				sum(d.total_single_gift_count)						as [cibernfp_totalsinglegiftscount],
				sum(d.total_regular_gift_count)						as [cibernfp_totalregulargiftscount],
				sum(d.cib_singlegivingstatus)						as [cib_singlegivingstatus],
				max(d.cib_nextbirthdaydate)	as cib_nextbirthdaydate,
				max(d.cib_nextpaymentanniversarydate) as [cib_nextpaymentanniversarydate],
				sum(d.cibernfp_totalsinglegiftsexcgiftaid) as [cibernfp_totalsinglegiftsexcgiftaid],
				sum(d.cibernfp_totalregulargiftsexcgiftaid) as [cibernfp_totalregulargiftsexcgiftaid],
				sum(d.cib_totalannualcumulativegift) as [cib_totalannualcumulativegift],
				max(d.cibernfp_datelastregulargift) as [cibernfp_datelastregulargift],
				max(d.cibernfp_datelastsinglegift) as [cibernfp_datelastsinglegift]


		from	(
			--Stats based on income
			--
			select	a.cibernfp_contactid, 
				SUM(a.Cibernfp_Amount_Base)		as total_income,
				COUNT(*)				as total_income_count,
				0.0					as TotalIncomeincgiftaid,			
				0.0					as incomelast12monthsincgiftaid,
				0.0					as incomelast12monthsexcgiftaid,
				0.0					as incomelast24monthsincgiftaid,			
				0.0					as incomelast24monthsexcgiftaid,
				0.0					as incomelast36monthsincgiftaid,
				0.0					as incomelast36monthsexcGiftAid,
				0.0					as total_single_gift_income,
				0.0					as total_single_gift_income_inc_tax,
				0					as total_single_gift_count,
				0.0					as total_regular_gift_income,
				0.0					as total_regular_gift_income_inc_tax,
				0					as total_regular_gift_count,
				0.0					as totalvoluntaryincomeexgiftaid,
				0					as totalvoluntaryincomeexgiftaidcount,
				0.0					as totalvoluntaryincomeincgiftaid,
				0					as totalvoluntaryincomeincgiftaidcount,
				MAX(a.cibernfp_DateReceived)		as last_income_date,
				0					as age,
				0					as volunteeringstatus,
				0					as donationacknowledgements,
				0					as directdebitmailingsasks,
				0					as giftaidmailings,
				0					as cib_singlegivingstatus,
				null				as cib_nextbirthdaydate,
				NULL as [cib_nextpaymentanniversarydate],
				NULL as [cibernfp_totalsinglegiftsexcgiftaid],
				NULL as [cibernfp_totalregulargiftsexcgiftaid],
				NULL as [cib_totalannualcumulativegift],
				NULL as [cibernfp_datelastregulargift],
				NULL as [cibernfp_datelastsinglegift]

			from		Syn_cibernfp_incomebatchline	as	a	(nolock)
			inner join	Syn_Cibernfp_incomebatch	as	b	(nolock)	on	b.Id	=	a.cibernfp_incomebatchid
													and	b.CiberNfp_batchtype		=	@batch_type_income
			where		a.cibernfp_ContactId is not null
			group by	a.cibernfp_contactid	

			union all

			--Stats based on income received in the last 12 months
			--
			select	a.cibernfp_contactid, 
				0.0							as total_income,
				0							as total_income_count,
				0.0							as TotalIncomeincgiftaid,			
				0.0							as incomelast12monthsincgiftaid,
				SUM(a.cibernfp_Amount_Base)				as incomelast12monthsexcgiftaid,
				0.0							as incomelast24monthsincgiftaid,			
				0.0							as incomelast24monthsexcgiftaid,
				0.0							as incomelast36monthsincgiftaid,
				0.0							as incomelast36monthsexcGiftAid,
				0.0							as total_single_gift_income,
				0.0							as total_single_gift_income_inc_tax,
				0							as total_single_gift_count,
				0.0							as total_regular_gift_income,
				0.0							as total_regular_gift_income_inc_tax,
				0							as total_regular_gift_count,
				0.0							as totalvoluntaryincomeexgiftaid,
				0							as totalvoluntaryincomeexgiftaidcount,
				0.0							as totalvoluntaryincomeincgiftaid,
				0							as totalvoluntaryincomeincgiftaidcount,
				null							as last_income_date,
				0							as age,
				0							as volunteeringstatus,
				0							as donationacknowledgements,
				0							as directdebitmailingsasks,
				0							as giftaidmailings,
				0					as cib_singlegivingstatus,
				null				as cib_nextbirthdaydate,
				NULL as [cib_nextpaymentanniversarydate],
				NULL as [cibernfp_totalsinglegiftsexcgiftaid],
				NULL as [cibernfp_totalregulargiftsexcgiftaid],
				NULL as [cib_totalannualcumulativegift],
				NULL as [cibernfp_datelastregulargift],
				NULL as [cibernfp_datelastsinglegift]
			from		Syn_cibernfp_incomebatchline		as	a (nolock)
			inner join	Syn_Cibernfp_incomebatch		as	b (nolock)	on	b.Id	= a.cibernfp_incomebatchid
													and	b.CiberNfp_batchtype		= @batch_type_income
			where		a.cibernfp_DateReceived > GETDATE()-365
			and		a.cibernfp_ContactId is not null
			group by	a.cibernfp_contactid	

			union all

			--Stats based on income received in the last 12 months - single gift allocation
			--
			select	a.cibernfp_contactid, 
				0.0							as total_income,
				0							as total_income_count,
				0.0							as TotalIncomeincgiftaid,			
				SUM(c.cibernfp_ValueInctax_Base)			as incomelast12monthsincgiftaid,
				0.0							as incomelast12monthsexcgiftaid,
				0.0							as incomelast24monthsincgiftaid,			
				0.0							as incomelast24monthsexcgiftaid,
				0.0							as incomelast36monthsincgiftaid,
				0.0							as incomelast36monthsexcGiftAid,
				0.0							as total_single_gift_income,
				0.0							as total_single_gift_income_inc_tax,
				0							as total_single_gift_count,
				0.0							as total_regular_gift_income,
				0.0							as total_regular_gift_income_inc_tax,
				0							as total_regular_gift_count,
				0.0							as totalvoluntaryincomeexgiftaid,
				0							as totalvoluntaryincomeexgiftaidcount,
				0.0							as totalvoluntaryincomeincgiftaid,
				0							as totalvoluntaryincomeincgiftaidcount,
				null							as last_income_date,
				0							as age,
				0							as volunteeringstatus,
				0							as donationacknowledgements,
				0							as directdebitmailingsasks,
				0							as giftaidmailings,
				0					as cib_singlegivingstatus,
				null				as cib_nextbirthdaydate,
				NULL as [cib_nextpaymentanniversarydate],
				NULL as [cibernfp_totalsinglegiftsexcgiftaid],
				NULL as [cibernfp_totalregulargiftsexcgiftaid],
				SUM(c.cibernfp_ValueInctax_Base) as [cib_totalannualcumulativegift],
				NULL as [cibernfp_datelastregulargift],
				NULL as [cibernfp_datelastsinglegift]
			from		Syn_cibernfp_incomebatchline		as	a (nolock)
			inner join	Syn_Cibernfp_incomebatch		as	b (nolock)	on	b.Id			= a.cibernfp_incomebatchid
													and	b.CiberNfp_batchtype				= @batch_type_income
			inner join	Syn_cibernfp_singlegiftallocation	as	c (nolock)	on	c.cibernfp_IncomeBatchLineId			= a.Id
			where		a.cibernfp_DateReceived > GETDATE()-365
			and		a.cibernfp_ContactId is not null
			group by	a.cibernfp_contactid

			union all

	
			--Stats based on income received in the last 24 months
			--
			select	a.cibernfp_contactid, 
				0.0							as total_income,
				0							as total_income_count,
				0.0							as TotalIncomeincgiftaid,			
				0.0							as incomelast12monthsincgiftaid,
				0.0							as incomelast12monthsexcgiftaid,
				0.0							as incomelast24monthsincgiftaid,			
				SUM(a.cibernfp_Amount_Base)				as incomelast24monthsexcgiftaid,
				0.0							as incomelast36monthsincgiftaid,
				0.0							as incomelast36monthsexcGiftAid,
				0.0							as total_single_gift_income,
				0.0							as total_single_gift_income_inc_tax,
				0							as total_single_gift_count,
				0.0							as total_regular_gift_income,
				0.0							as total_regular_gift_income_inc_tax,
				0							as total_regular_gift_count,
				0.0							as totalvoluntaryincomeexgiftaid,
				0							as totalvoluntaryincomeexgiftaidcount,
				0.0							as totalvoluntaryincomeincgiftaid,
				0							as totalvoluntaryincomeincgiftaidcount,
				null							as last_income_date,
				0							as age,
				0							as volunteeringstatus,
				0							as donationacknowledgements,
				0							as directdebitmailingsasks,
				0							as giftaidmailings,
				0					as cib_singlegivingstatus,
				null				as cib_nextbirthdaydate,
				NULL as [cib_nextpaymentanniversarydate],
				NULL as [cibernfp_totalsinglegiftsexcgiftaid],
				NULL as [cibernfp_totalregulargiftsexcgiftaid],
				NULL as [cib_totalannualcumulativegift],
				NULL as [cibernfp_datelastregulargift],
				NULL as [cibernfp_datelastsinglegift]
			from		Syn_cibernfp_incomebatchline	as	a (nolock)
			inner join	Syn_Cibernfp_incomebatch	as	b (nolock)	on	b.Id	= a.cibernfp_incomebatchid
												and	b.CiberNfp_batchtype		= @batch_type_income
			where		a.cibernfp_DateReceived > GETDATE()-730
			and		a.cibernfp_ContactId is not null
			group by	a.cibernfp_contactid	

			union all

			--Stats based on income received in the last 24 months - single gift allocation
			--
			select	a.cibernfp_contactid, 
				0.0							as total_income,
				0							as total_income_count,
				0.0							as TotalIncomeincgiftaid,			
				0.0							as incomelast12monthsincgiftaid,
				0.0							as incomelast12monthsexcgiftaid,
				SUM(c.cibernfp_ValueInctax_Base)			as incomelast24monthsincgiftaid,			
				0.0							as incomelast24monthsexcgiftaid,
				0.0							as incomelast36monthsincgiftaid,
				0.0							as incomelast36monthsexcGiftAid,
				0.0							as total_single_gift_income,
				0.0							as total_single_gift_income_inc_tax,
				0							as total_single_gift_count,
				0.0							as total_regular_gift_income,
				0.0							as total_regular_gift_income_inc_tax,
				0							as total_regular_gift_count,
				0.0							as totalvoluntaryincomeexgiftaid,
				0							as totalvoluntaryincomeexgiftaidcount,
				0.0							as totalvoluntaryincomeincgiftaid,
				0							as totalvoluntaryincomeincgiftaidcount,
				null							as last_income_date,
				0							as age,
				0							as volunteeringstatus,
				0							as donationacknowledgements,
				0							as directdebitmailingsasks,
				0							as giftaidmailings,
				0					as cib_singlegivingstatus,
				null				as cib_nextbirthdaydate,
				NULL as [cib_nextpaymentanniversarydate],
				NULL as [cibernfp_totalsinglegiftsexcgiftaid],
				NULL as [cibernfp_totalregulargiftsexcgiftaid],
				NULL as [cib_totalannualcumulativegift],
				NULL as [cibernfp_datelastregulargift],
				NULL as [cibernfp_datelastsinglegift]
			from		Syn_cibernfp_incomebatchline			as	a (nolock)
			inner join	Syn_Cibernfp_incomebatch				as	b (nolock)	on	b.Id		= a.cibernfp_incomebatchid
																and	b.CiberNfp_batchtype			= @batch_type_income
			inner join Syn_cibernfp_singlegiftallocation		as	c (nolock)		on	c.cibernfp_IncomeBatchLineId		= a.Id
			where		a.cibernfp_DateReceived > GETDATE()-730
			and		a.cibernfp_ContactId is not null
			group by	a.cibernfp_contactid	

			union all

			
			--Stats based on income received in the last 36 months
			--
			select	a.cibernfp_contactid, 
				0.0							as total_income,
				0							as total_income_count,
				0.0							as TotalIncomeincgiftaid,			
				0.0							as incomelast12monthsincgiftaid,
				0.0							as incomelast12monthsexcgiftaid,
				0.0							as incomelast24monthsincgiftaid,			
				0.0							as incomelast24monthsexcgiftaid,
				0.0							as incomelast36monthsincgiftaid,
				SUM(a.cibernfp_Amount_Base)				as incomelast36monthsexcGiftAid,
				0.0							as total_single_gift_income,
				0.0							as total_single_gift_income_inc_tax,
				0							as total_single_gift_count,
				0.0							as total_regular_gift_income,
				0.0							as total_regular_gift_income_inc_tax,
				0							as total_regular_gift_count,
				0.0							as totalvoluntaryincomeexgiftaid,
				0							as totalvoluntaryincomeexgiftaidcount,
				0.0							as totalvoluntaryincomeincgiftaid,
				0							as totalvoluntaryincomeincgiftaidcount,
				null							as last_income_date,
				0							as age,
				0							as volunteeringstatus,
				0							as donationacknowledgements,
				0							as directdebitmailingsasks,
				0							as giftaidmailings,
				0					as cib_singlegivingstatus,
				null				as cib_nextbirthdaydate,
				NULL as [cib_nextpaymentanniversarydate],
				NULL as [cibernfp_totalsinglegiftsexcgiftaid],
				NULL as [cibernfp_totalregulargiftsexcgiftaid],
				NULL as [cib_totalannualcumulativegift],
				NULL as [cibernfp_datelastregulargift],
				NULL as [cibernfp_datelastsinglegift]
			from		Syn_cibernfp_incomebatchline	as	a (nolock)
			inner join	Syn_Cibernfp_incomebatch		as	b (nolock)	on	b.Id	= a.cibernfp_incomebatchid
														and	b.CiberNfp_batchtype		= @batch_type_income
			where		a.cibernfp_DateReceived > GETDATE()-1095
			and		a.cibernfp_ContactId is not null
			group by	a.cibernfp_contactid	

			union all

			--Stats based on income received in the last 36 months - single gift allocation
			--
			select	a.cibernfp_contactid, 
				0.0							as total_income,
				0							as total_income_count,
				0.0							as TotalIncomeincgiftaid,			
				0.0							as incomelast12monthsincgiftaid,
				0.0							as incomelast12monthsexcgiftaid,
				0.0							as incomelast24monthsincgiftaid,			
				0.0							as incomelast24monthsexcgiftaid,
				SUM(c.cibernfp_ValueInctax_Base)			as incomelast36monthsincgiftaid,
				0.0							as incomelast36monthsexcGiftAid,
				0.0							as total_single_gift_income,
				0.0							as total_single_gift_income_inc_tax,
				0							as total_single_gift_count,
				0.0							as total_regular_gift_income,
				0.0							as total_regular_gift_income_inc_tax,
				0							as total_regular_gift_count,
				0.0							as totalvoluntaryincomeexgiftaid,
				0							as totalvoluntaryincomeexgiftaidcount,
				0.0							as totalvoluntaryincomeincgiftaid,
				0							as totalvoluntaryincomeincgiftaidcount,
				null							as last_income_date,
				0							as age,
				0							as volunteeringstatus,
				0							as donationacknowledgements,
				0							as directdebitmailingsasks,
				0							as giftaidmailings,
				0					as cib_singlegivingstatus,
				null				as cib_nextbirthdaydate,
				NULL as [cib_nextpaymentanniversarydate],
				NULL as [cibernfp_totalsinglegiftsexcgiftaid],
				NULL as [cibernfp_totalregulargiftsexcgiftaid],
				NULL as [cib_totalannualcumulativegift],
				NULL as [cibernfp_datelastregulargift],
				NULL as [cibernfp_datelastsinglegift]
			from		Syn_cibernfp_incomebatchline			as	a (nolock)
			inner join	Syn_Cibernfp_incomebatch				as	b (nolock)	on	b.Id		= a.cibernfp_incomebatchid
																and	b.CiberNfp_batchtype			= @batch_type_income
			inner join	Syn_cibernfp_singlegiftallocation			as	c (nolock)	on	c.cibernfp_IncomeBatchLineId		= a.Id
			where		a.cibernfp_DateReceived > GETDATE()-1095
			and		a.cibernfp_ContactId is not null
			group by	a.cibernfp_contactid	

			union all

			--Stats based on single gifts (part of voluntary income)
			--
			select	a.cibernfp_contactid, 
				0.0								as total_income,
				0								as total_income_count,
				0.0								as TotalIncomeincgiftaid,			
				0.0								as incomelast12monthsincgiftaid,
				0.0								as incomelast12monthsexcgiftaid,
				0.0								as incomelast24monthsincgiftaid,			
				0.0								as incomelast24monthsexcgiftaid,
				0.0								as incomelast36monthsincgiftaid,
				0.0								as incomelast36monthsexcGiftAid,
				SUM(c.Cibernfp_ValueexcTax_Base)				as total_single_gift_income,
				SUM(c.Cibernfp_ValueincTax_Base)				as total_single_gift_income_inc_tax,
				COUNT(*)							as total_single_gift_count,
				0.0								as total_regular_gift_income,
				0.0								as total_regular_gift_income_inc_tax,
				0								as total_regular_gift_count,
				0.0								as totalvoluntaryincomeexgiftaid,
				0								as totalvoluntaryincomeexgiftaidcount,
				0.0								as totalvoluntaryincomeincgiftaid,
				0								as totalvoluntaryincomeincgiftaidcount,
				null								as last_income_date,
				null								as age,
				0								as volunteeringstatus,
				0								as donationacknowledgements,
				0								as directdebitmailingsasks,
				0								as giftaidmailings,
				0					as cib_singlegivingstatus,
				null				as cib_nextbirthdaydate,
				NULL as [cib_nextpaymentanniversarydate],
				SUM(c.Cibernfp_ValueexcTax_Base) as [cibernfp_totalsinglegiftsexcgiftaid],
				NULL as [cibernfp_totalregulargiftsexcgiftaid],
				NULL as [cib_totalannualcumulativegift],
				NULL as [cibernfp_datelastregulargift],
				max(c.cibernfp_DateReceived) as [cibernfp_datelastsinglegift]
			from		Syn_cibernfp_incomebatchline		as	a (nolock)
			inner join	Syn_Cibernfp_incomebatch			as	b (nolock)	on	b.Id		= a.cibernfp_incomebatchid
															and	b.CiberNfp_batchtype			= @batch_type_income
			inner join	Syn_cibernfp_singlegiftallocation as c (nolock)			on	c.cibernfp_IncomeBatchLineId		= a.Id
			where		a.cibernfp_ContactId is not null and c.cib_gifttype = 912800000 --single
			group by	a.cibernfp_contactid

			union all

			----Stats based on regular gifts (part of voluntary income)
			----
			select	a.cibernfp_contactid, 
				0.0								as total_income,
				0								as total_income_count,
				0.0								as TotalIncomeincgiftaid,			
				0.0								as incomelast12monthsincgiftaid,
				0.0								as incomelast12monthsexcgiftaid,
				0.0								as incomelast24monthsincgiftaid,			
				0.0								as incomelast24monthsexcgiftaid,
				0.0								as incomelast36monthsincgiftaid,
				0.0								as incomelast36monthsexcGiftAid,
				0.0								as total_single_gift_income,
				0.0								as total_single_gift_income_inc_tax,
				0								as total_single_gift_count,
				SUM(c.Cibernfp_ValueexcTax_Base)				as total_regular_gift_income,
				SUM(c.Cibernfp_ValueincTax_Base)				as total_regular_gift_income_inc_tax,
				COUNT(*)							as total_regular_gift_count,
				0.0								as totalvoluntaryincomeexgiftaid,
				0								as totalvoluntaryincomeexgiftaidcount,
				0.0								as totalvoluntaryincomeincgiftaid,
				0								as totalvoluntaryincomeincgiftaidcount,
				null								as last_income_date,
				null								as age,
				0								as volunteeringstatus,
				0								as donationacknowledgements,
				0								as directdebitmailingsasks,
				0								as giftaidmailings,
				0					as cib_singlegivingstatus,
				null				as cib_nextbirthdaydate,
				NULL as [cib_nextpaymentanniversarydate],
				NULL as [cibernfp_totalsinglegiftsexcgiftaid],
				SUM(c.Cibernfp_ValueexcTax_Base) as [cibernfp_totalregulargiftsexcgiftaid],
				NULL as [cib_totalannualcumulativegift],
				max(c.cibernfp_DateReceived) as [cibernfp_datelastregulargift],
				NULL as [cibernfp_datelastsinglegift]
			from		Syn_cibernfp_incomebatchline	as	a (nolock)
			inner join	Syn_Cibernfp_incomebatch	as	b (nolock)	on	b.Id		= a.cibernfp_incomebatchid
																									and	b.CiberNfp_batchtype			= @batch_type_income
			inner join	Syn_cibernfp_singlegiftallocation	as c (nolock)	on	c.cibernfp_IncomeBatchLineId	= a.id
			where		a.cibernfp_ContactId is not null and c.cib_gifttype = 912800002
			group by	a.cibernfp_contactid
			
			union all
			
			--Stats based on contact data and preferences
			--
			select	a.Id as cibernfp_contactid, 
				0.0																						as total_income,
				0																						as total_income_count,
				0.0																						as TotalIncomeincgiftaid,			
				0.0																						as incomelast12monthsincgiftaid,
				0.0																						as incomelast12monthsexcgiftaid,
				0.0																						as incomelast24monthsincgiftaid,			
				0.0																						as incomelast24monthsexcgiftaid,
				0.0																						as incomelast36monthsincgiftaid,
				0.0																						as incomelast36monthsexcGiftAid,
				0.0																						as total_single_gift_income,
				0.0																						as total_single_gift_income_inc_tax,
				0																						as total_single_gift_count,
				0.0																						as total_regular_gift_income,
				0.0																						as total_regular_gift_income_inc_tax,
				0																						as total_regular_gift_count,
				0.0																						as totalvoluntaryincomeexgiftaid,
				0																						as totalvoluntaryincomeexgiftaidcount,
				0.0																						as totalvoluntaryincomeincgiftaid,
				0																						as totalvoluntaryincomeincgiftaidcount,
				null																						as last_income_date,
				dbo.fn_GetAgeFromDateOfBirth (a.BirthDate)																	as age,
				(CASE 
				WHEN 
				(select			COUNT(c.statuscode) 
				from			Syn_Connection					c 
				inner join		Syn_ConnectionRole				cr	on	c.Record2RoleId			=	cr.Id 
				where			c.statuscode						=		1 
				and				cr.Name								like	'%branch%'
				and				c.Record1Id							=		a.Id
				and				c.StatusCode						=	2)	>	0 
				THEN			814370001
				WHEN 
				(select			COUNT(c.statuscode)
				from			Syn_Connection					c 
				inner join		Syn_ConnectionRole				cr	on	c.Record2RoleId			=	cr.Id 
				where			c.statuscode						=		2 
				and				cr.Name								like	'%branch%'
				and				c.Record1Id							=		a.Id
				and				c.StatusCode						=	1)	>	0
				THEN			814370000 
				END)					 																	as volunteeringstatus,
				case
				when 
				(select			count(*)
				from			syn_cibernfp_preference	as p
				where			p.cibernfp_contactid = a.id
				and			p.cibernfp_startdate <= @today
				and			(p.cibernfp_enddate >= @today or p.cibernfp_enddate is null)
				and			p.cibernfp_preference = 322880009) > 1
				then			1
				else			0
				end																						as donationacknowledgements,
				case
				when 
				(select			count(*)
				from			syn_cibernfp_preference	as p
				where			p.cibernfp_contactid = a.id
				and			p.cibernfp_startdate <= @today
				and			(p.cibernfp_enddate >= @today or p.cibernfp_enddate is null)
				and			p.cibernfp_preference = 322880014) > 1
				then			1
				else			0
				end																						as directdebitmailingsasks,
				case
				when 
				(select			count(*)
				from			syn_cibernfp_preference	as p
				where			p.cibernfp_contactid = a.id
				and			p.cibernfp_startdate <= @today
				and			(p.cibernfp_enddate >= @today or p.cibernfp_enddate is null)
				and			p.cibernfp_preference = 322880004) > 1
				then			1
				else			0
				end																						as giftaidmailings,
				0					as cib_singlegivingstatus,
				dbo.fn_GetNextBirthday (a.BirthDate) as cib_nextbirthdaydate,
				NULL as [cib_nextpaymentanniversarydate],
				NULL as [cibernfp_totalsinglegiftsexcgiftaid],
				NULL as [cibernfp_totalregulargiftsexcgiftaid],
				NULL as [cib_totalannualcumulativegift],
				NULL as [cibernfp_datelastregulargift],
				NULL as [cibernfp_datelastsinglegift]
				from			Syn_Contact a (nolock)	
				
				
				union all

				--single giving status and single giving journey stage changes

				select	
				a.Id							as cibernfp_contactid,
				0.0								as total_income,
				0								as total_income_count,
				0.0								as TotalIncomeincgiftaid,			
				0.0								as incomelast12monthsincgiftaid,
				0.0								as incomelast12monthsexcgiftaid,
				0.0								as incomelast24monthsincgiftaid,			
				0.0								as incomelast24monthsexcgiftaid,
				0.0								as incomelast36monthsincgiftaid,
				0.0								as incomelast36monthsexcGiftAid,
				0.0								as total_single_gift_income,
				0.0								as total_single_gift_income_inc_tax,
				0								as total_single_gift_count,
				0.0								as total_regular_gift_income,
				0.0								as total_regular_gift_income_inc_tax,
				0								as total_regular_gift_count,
				0.0								as totalvoluntaryincomeexgiftaid,
				0								as totalvoluntaryincomeexgiftaidcount,
				0.0								as totalvoluntaryincomeincgiftaid,
				0								as totalvoluntaryincomeincgiftaidcount,
				null							as last_income_date,
				null							as age,
				0								as volunteeringstatus,
				0								as donationacknowledgements,
				0								as directdebitmailingsasks,
				0								as giftaidmailings,		
				CASE
				WHEN DATEDIFF (dd, max(sga.cibernfp_DateReceived), GETDATE ()) >= @single_giving_lapsed_period
				THEN 912800002 --Lapsed
				ELSE 912800000
				END			as cib_singlegivingstatus,
				null				as cib_nextbirthdaydate,
				NULL as [cib_nextpaymentanniversarydate],
				NULL as [cibernfp_totalsinglegiftsexcgiftaid],
				NULL as [cibernfp_totalregulargiftsexcgiftaid],
				NULL as [cib_totalannualcumulativegift],
				NULL as [cibernfp_datelastregulargift],
				NULL as [cibernfp_datelastsinglegift]
				from			Syn_Contact a (nolock)
				inner join Syn_cibernfp_singlegiftallocation sga on sga.cibernfp_ContactId = a.Id
				where sga.cib_gifttype = 912800000 --single
				group by a.Id
					
				union all	 
				-- next payment anniversary date
				select 
					ppd.cibernfp_ContactId as cibernfp_contactid,
					0.0																						as total_income,
					0																						as total_income_count,
					0.0																						as TotalIncomeincgiftaid,			
					0.0																						as incomelast12monthsincgiftaid,
					0.0																						as incomelast12monthsexcgiftaid,
					0.0																						as incomelast24monthsincgiftaid,			
					0.0																						as incomelast24monthsexcgiftaid,
					0.0																						as incomelast36monthsincgiftaid,
					0.0																						as incomelast36monthsexcGiftAid,
					0.0																						as total_single_gift_income,
					0.0																						as total_single_gift_income_inc_tax,
					0																						as total_single_gift_count,
					0.0																						as total_regular_gift_income,
					0.0																						as total_regular_gift_income_inc_tax,
					0																						as total_regular_gift_count,
					0.0																						as totalvoluntaryincomeexgiftaid,
					0																						as totalvoluntaryincomeexgiftaidcount,
					0.0																						as totalvoluntaryincomeincgiftaid,
					0																						as totalvoluntaryincomeincgiftaidcount,
					null																						as last_income_date,
					0 as age,
					null as volunteeringstatus,
					0 as donationacknowledgements,
					0 as directdebitmailingsasks,
					0 as giftaidmailings,
					0					as cib_singlegivingstatus,
					null as cib_nextbirthdaydate,
					max(dbo.fn_GetNextBirthday (ppd.cibernfp_pledgedate)) as [cib_nextpaymentanniversarydate],
					NULL as [cibernfp_totalsinglegiftsexcgiftaid],
					NULL as [cibernfp_totalregulargiftsexcgiftaid],
					NULL as [cib_totalannualcumulativegift],
					NULL as [cibernfp_datelastregulargift],
					NULL as [cibernfp_datelastsinglegift]
				from			Syn_Contact a (nolock)
					inner join Syn_cibernfp_pledgepaymentdetail ppd on ppd.cibernfp_ContactId = a.Id
				where ppd.cibernfp_PledgeStatus = 814370002 --active
				group by ppd.cibernfp_ContactId

												
			)	as			d
		group by				d.cibernfp_contactid
		
		
		

		--------------------------------------------------------------------------------
		--Populate the temp table with the current values
		--------------------------------------------------------------------------------
		--
		update	[#ContactAnalysis]
		set	[old_cibernfp_totalincome]				= b.[cibernfp_totalincome],					
			[old_cibernfp_totalincomecount]				= b.[cibernfp_totalincomecount],					
			[old_cibernfp_totalvoluntaryincomeexcgiftaid]		= b.[cibernfp_totalvoluntaryincomeexcgiftaid],			
			[old_cibernfp_totalvoluntaryincomeexcgiftaidcount]	= b.[cibernfp_totalvoluntaryincomeexcgiftaidcount],		
			[old_cibernfp_totalvoluntaryincomeincgiftaid]		= b.[cibernfp_totalvoluntaryincomeincgiftaid],			
			[old_cibernfp_totalvoluntaryincomeincgiftaidcount]	= b.[cibernfp_totalvoluntaryincomeincgiftaidcount],		
			[old_cibernfp_datelastincome]				= b.[cibernfp_datelastincome],					
			[old_cibernfp_age]					= b.[cibernfp_age],				
			[old_cibernfp_volunteeringstatus]			= b.[cibernfp_volunteeringstatus],				
			[old_cibernfp_totalincomeincgiftaid]			= b.[cibernfp_totalincomeincgiftaid],				
			[old_cibernfp_MajorDonorStatus]				= b.[cibernfp_MajorDonorStatus],
			[old_cibernfp_donationacknowledgements]			= b.[cibernfp_donationacknowledgements],          
			[old_cibernfp_directdebitmailingsasks]             	= b.[cibernfp_directdebitmailingsasks],           
			[old_cibernfp_giftaidmailings]                     	= b.[cibernfp_giftaidmailings],
			[old_cibernfp_totalsinglegiftscount]				= b.[cibernfp_TotalSingleGiftsCount],
			[old_cibernfp_totalregulargiftscount]				= b.[cibernfp_totalregulargiftscount],
			[old_cib_singlegivingstatus]						= b.[cib_singlegivingstatus],
			[old_cib_nextpaymentanniversarydate]				= b.[cib_nextpaymentanniversarydate],
			[old_cibernfp_totalsinglegiftsexcgiftaid]			= b.[cibernfp_totalsinglegiftsexcgiftaid],
			[old_cibernfp_totalregulargiftsexcgiftaid]			= b.[cibernfp_totalregulargiftsexcgiftaid],
			[old_cib_totalannualcumulativegift]					= b.[cib_totalannualcumulativegift],
			[old_cibernfp_datelastregulargift]					= b.[cibernfp_datelastregulargift],
			[old_cibernfp_datelastsinglegift]					= b.[cibernfp_datelastsinglegift]
			                       				
		from	[#ContactAnalysis] as a, Syn_Contact as b
		where	a.id = b.Id

		
		--------------------------------------------------------------------------------
		--Delete records were no change has taken place
		--------------------------------------------------------------------------------
		--
		delete 
		from	[#ContactAnalysis]	
		where	checksum(
			 [cibernfp_totalincome]					
			,[cibernfp_totalincomecount]				
			,[cibernfp_totalvoluntaryincomeexcgiftaid]		
			,[cibernfp_totalvoluntaryincomeexcgiftaidcount]		
			,[cibernfp_totalvoluntaryincomeincgiftaid]		
			,[cibernfp_totalvoluntaryincomeincgiftaidcount]		
			,[cibernfp_datelastincome]				
			,[cibernfp_age]			
			,[cibernfp_volunteeringstatus]				
			,[cibernfp_totalincomeincgiftaid]			
			,[cibernfp_donationacknowledgements]			
			,[cibernfp_directdebitmailingsasks]			
			,[cibernfp_giftaidmailings]
			,[cibernfp_totalsinglegiftscount]
			,[cibernfp_totalregulargiftscount]
			,[cib_singlegivingstatus]
			)
		=				
		checksum(
			 [old_cibernfp_totalincome]				
			,[old_cibernfp_totalincomecount]				
			,[old_cibernfp_totalvoluntaryincomeexcgiftaid]		
			,[old_cibernfp_totalvoluntaryincomeexcgiftaidcount]	
			,[old_cibernfp_totalvoluntaryincomeincgiftaid]		
			,[old_cibernfp_totalvoluntaryincomeincgiftaidcount]	
			,[old_cibernfp_datelastincome]				
			,[old_cibernfp_age]		
			,[old_cibernfp_volunteeringstatus]			
			,[old_cibernfp_totalincomeincgiftaid]			
			,[old_cibernfp_donationacknowledgements]			
			,[old_cibernfp_directdebitmailingsasks]			
			,[old_cibernfp_giftaidmailings]
			,[old_cibernfp_totalsinglegiftscount]
			,[old_cibernfp_totalregulargiftscount]
			,[old_cib_singlegivingstatus]
			)				
		
		--------------------------------------------------------------------------------
		--Populate the contact analysis table from the temp table
		--------------------------------------------------------------------------------
		--
		INSERT INTO [dbo].[ContactAnalysis]
			(
			[id],
			[cibernfp_totalincome],
			[cibernfp_totalincomecount],
			[cibernfp_totalvoluntaryincomeexcgiftaid],
			[cibernfp_totalvoluntaryincomeexcgiftaidcount],
			[cibernfp_totalvoluntaryincomeincgiftaid],
			[cibernfp_totalvoluntaryincomeincgiftaidcount],
			[cibernfp_datelastincome],
			[cibernfp_age],
			[cibernfp_volunteeringstatus],
			[cibernfp_totalincomeincgiftaid],
			[cibernfp_majordonorstatus],
			[cibernfp_donationacknowledgements],
			[cibernfp_directdebitmailingsasks],
			[cibernfp_giftaidmailings],
			[cibernfp_totalsinglegiftscount],
			[cibernfp_totalregulargiftscount],
			[cib_singlegivingstatus],
			[cib_nextbirthdaydate],
			[cib_nextpaymentanniversarydate],
			[cibernfp_totalsinglegiftsexcgiftaid],
			[cibernfp_totalregulargiftsexcgiftaid],
			[cib_totalannualcumulativegift],
			[cibernfp_datelastregulargift],
			[cibernfp_datelastsinglegift]
			)
		select	[id],
			[cibernfp_totalincome],
			[cibernfp_totalincomecount],
			[cibernfp_totalvoluntaryincomeexcgiftaid],
			[cibernfp_totalvoluntaryincomeexcgiftaidcount],
			[cibernfp_totalvoluntaryincomeincgiftaid],
			[cibernfp_totalvoluntaryincomeincgiftaidcount],
			[cibernfp_datelastincome],
			[cibernfp_age],
			case [cibernfp_volunteeringstatus]		when 0 then null else [cibernfp_volunteeringstatus] end,
			[cibernfp_totalincomeincgiftaid],
			case [cibernfp_majordonorstatus]		when 0 then null else [cibernfp_MajorDonorStatus] end,
			[cibernfp_donationacknowledgements],
			[cibernfp_directdebitmailingsasks],
			[cibernfp_giftaidmailings],
			[cibernfp_totalsinglegiftscount],
			[cibernfp_totalregulargiftscount],
			case [cib_singlegivingstatus] when 0 then null else [cib_singlegivingstatus] end,
			[cib_nextbirthdaydate],
			[cib_nextpaymentanniversarydate],
			[cibernfp_totalsinglegiftsexcgiftaid],
			[cibernfp_totalregulargiftsexcgiftaid],
			[cib_totalannualcumulativegift],
			[cibernfp_datelastregulargift],
			[cibernfp_datelastsinglegift]
		from	[#ContactAnalysis]
		
		where	id	is not null 

		--------------------------------------------------------------------------------
		--Drop temp table 
		--------------------------------------------------------------------------------
		--
		drop table [#ContactAnalysis]

	end


end






GO


