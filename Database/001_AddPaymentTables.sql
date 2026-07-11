-- =============================================================================
-- Migration: Add Payment and PaymentAllocation tables
-- =============================================================================

-- ── Payment ──────────────────────────────────────────────────────────────────
CREATE TABLE Payment
(
    PaymentID         INT           NOT NULL IDENTITY(1,1) PRIMARY KEY,
    AccountHolderID   INT           NOT NULL REFERENCES AccountHolder(AccountHolderID),
    Amount            DECIMAL(10,2) NOT NULL CHECK (Amount > 0),
    UnallocatedAmount DECIMAL(10,2) NOT NULL DEFAULT 0 CHECK (UnallocatedAmount >= 0),
    PaymentDate       DATE          NOT NULL,
    Source            NVARCHAR(20)  NOT NULL DEFAULT 'Manual',   -- Manual | QuickPay
    Reference         NVARCHAR(100) NULL,
    Notes             NVARCHAR(500) NULL,
    CreatedAt         DATETIME2     NOT NULL DEFAULT GETUTCDATE()
);

-- ── PaymentAllocation ─────────────────────────────────────────────────────────
-- Tracks which portion of each payment was applied to which invoice.
CREATE TABLE PaymentAllocation
(
    AllocationID   INT           NOT NULL IDENTITY(1,1) PRIMARY KEY,
    PaymentID      INT           NOT NULL REFERENCES Payment(PaymentID),
    InvoiceID      INT           NOT NULL REFERENCES Invoice(InvoiceID),
    AmountApplied  DECIMAL(10,2) NOT NULL CHECK (AmountApplied > 0),
    CreatedAt      DATETIME2     NOT NULL DEFAULT GETUTCDATE(),

    CONSTRAINT UQ_PaymentAllocation_Payment_Invoice UNIQUE (PaymentID, InvoiceID)
);

-- ── Indexes ───────────────────────────────────────────────────────────────────
CREATE INDEX IX_Payment_AccountHolderID ON Payment (AccountHolderID);
CREATE INDEX IX_PaymentAllocation_PaymentID ON PaymentAllocation (PaymentID);
CREATE INDEX IX_PaymentAllocation_InvoiceID ON PaymentAllocation (InvoiceID);
