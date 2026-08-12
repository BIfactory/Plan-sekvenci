-- Faze 3 (PRD 2) - audit log editaci provedenych pres appku "Plan sekvenci"
-- (checkbox fixed, toggle selected, zapis duvodu/poznamky - obrazovky Plan sekvenci
-- i Vyhodnoceni). Appka databazi BI_APP nevlastni a nepouziva EF Core migrace
-- (viz PRD 4.1) - tuto tabulku je proto potreba vytvorit rucne (DBA / vlastnik BI_APP)
-- pred nasazenim verze appky, ktera audit log zapisuje.
--
-- Appka tuto tabulku plne vlastni (analogicky t_log_powerapp) - pouze do ni zapisuje
-- (insert), nikdy needituje ani nemaze existujici radky.

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[t_workplan_audit_log](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[record_date] [datetime] NOT NULL,
	[user] [nvarchar](200) NOT NULL,
	[action] [nvarchar](50) NOT NULL,
	[entity_type] [nvarchar](20) NOT NULL,
	[entity_id] [nvarchar](100) NOT NULL,
	[node] [nvarchar](150) NULL,
	[old_value] [nvarchar](600) NULL,
	[new_value] [nvarchar](600) NULL,
 CONSTRAINT [PK_t_workplan_audit_log] PRIMARY KEY CLUSTERED
(
	[id] ASC
) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE INDEX [IX_t_workplan_audit_log_entity] ON [dbo].[t_workplan_audit_log]
(
	[entity_type] ASC,
	[entity_id] ASC
)
GO
