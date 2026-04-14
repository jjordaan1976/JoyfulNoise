-- Magic link access tokens for Student Portal and Account Holder Portal.
-- Each row grants one entity (Student or AccountHolder) access to its portal.
-- The teacher creates the link from the admin portal; the token travels in the URL.

CREATE TABLE MagicLink
(
    MagicLinkID INT            IDENTITY(1,1) NOT NULL,
    Token       UNIQUEIDENTIFIER             NOT NULL CONSTRAINT DF_MagicLink_Token DEFAULT NEWID(),
    LinkType    NVARCHAR(20)                 NOT NULL,   -- 'Student' | 'AccountHolder'
    EntityID    INT                          NOT NULL,   -- StudentID or AccountHolderID
    CreatedAt   DATETIME                     NOT NULL CONSTRAINT DF_MagicLink_CreatedAt DEFAULT GETUTCDATE(),
    ExpiresAt   DATETIME                         NULL,   -- NULL = never expires
    IsActive    BIT                          NOT NULL CONSTRAINT DF_MagicLink_IsActive DEFAULT 1,

    CONSTRAINT PK_MagicLink PRIMARY KEY CLUSTERED (MagicLinkID),
    CONSTRAINT CK_MagicLink_LinkType CHECK (LinkType IN ('Student', 'AccountHolder'))
);

CREATE UNIQUE NONCLUSTERED INDEX IX_MagicLink_Token ON MagicLink (Token);
CREATE        NONCLUSTERED INDEX IX_MagicLink_EntityID ON MagicLink (LinkType, EntityID);
