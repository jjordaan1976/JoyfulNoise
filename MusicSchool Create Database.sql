-- ============================================================
--  Music School Lesson Tracking System
--  MSSQL Schema  |  Data store only — no views or procedures
-- ============================================================

-- ------------------------------------------------------------
--  1. Teacher
-- ------------------------------------------------------------
CREATE TABLE Teacher (
    TeacherID   INT           NOT NULL IDENTITY(1,1),
    Name        NVARCHAR(100) NOT NULL,
    Email       NVARCHAR(150) NOT NULL,
    Phone       NVARCHAR(30)  NULL,
    IsActive    BIT           NOT NULL DEFAULT 1,
    CreatedAt   DATETIME2     NOT NULL DEFAULT SYSUTCDATETIME(),

    CONSTRAINT PK_Teacher       PRIMARY KEY (TeacherID),
    CONSTRAINT UQ_Teacher_Email UNIQUE      (Email)
);


-- ------------------------------------------------------------
--  2. AccountHolder
--     Contracted by a teacher. Billing party for students.
-- ------------------------------------------------------------
CREATE TABLE AccountHolder (
    AccountHolderID INT           NOT NULL IDENTITY(1,1),
    TeacherID       INT           NOT NULL,
    FirstName       NVARCHAR(100) NOT NULL,
    LastName        NVARCHAR(100) NOT NULL,
    Email           NVARCHAR(150) NOT NULL,
    Phone           NVARCHAR(30)  NULL,
    BillingAddress  NVARCHAR(300) NULL,
    IsActive        BIT           NOT NULL DEFAULT 1,
    CreatedAt       DATETIME2     NOT NULL DEFAULT SYSUTCDATETIME(),

    CONSTRAINT PK_AccountHolder       PRIMARY KEY (AccountHolderID),
    CONSTRAINT FK_AccountHolder_Teacher
        FOREIGN KEY (TeacherID) REFERENCES Teacher(TeacherID)
);


-- ------------------------------------------------------------
--  3. Student
--     Enrolled by an account holder.
--     IsAccountHolder = 1 when the same individual fills both roles.
-- ------------------------------------------------------------
CREATE TABLE Student (
    StudentID       INT           NOT NULL IDENTITY(1,1),
    AccountHolderID INT           NOT NULL,
    FirstName       NVARCHAR(100) NOT NULL,
    LastName        NVARCHAR(100) NOT NULL,
    DateOfBirth     DATE          NULL,
    IsAccountHolder BIT           NOT NULL DEFAULT 0,
    IsActive        BIT           NOT NULL DEFAULT 1,
    CreatedAt       DATETIME2     NOT NULL DEFAULT SYSUTCDATETIME(),

    CONSTRAINT PK_Student PRIMARY KEY (StudentID),
    CONSTRAINT FK_Student_AccountHolder
        FOREIGN KEY (AccountHolderID) REFERENCES AccountHolder(AccountHolderID)
);


-- ------------------------------------------------------------
--  4. LessonType
--     Defines available durations and their base price.
--     The application layer applies teacher discounts on top.
-- ------------------------------------------------------------
CREATE TABLE LessonType (
    LessonTypeID       INT            NOT NULL IDENTITY(1,1),
    DurationMinutes    INT            NOT NULL,
    BasePricePerLesson DECIMAL(10, 2) NOT NULL,
    IsActive           BIT            NOT NULL DEFAULT 1,

    CONSTRAINT PK_LessonType          PRIMARY KEY (LessonTypeID),
    CONSTRAINT UQ_LessonType_Duration UNIQUE      (DurationMinutes),
    CONSTRAINT CK_LessonType_Duration
        CHECK (DurationMinutes IN (30, 45, 60)),
    CONSTRAINT CK_LessonType_Price
        CHECK (BasePricePerLesson >= 0)
);


-- ------------------------------------------------------------
--  5. LessonBundle
--     Purchased once per year per student.
--     PricePerLesson stores the agreed (possibly discounted) rate.
--     TotalLessons must be divisible by 4 (one per quarter).
--     QuarterSize is a persisted computed column for convenience.
-- ------------------------------------------------------------
CREATE TABLE LessonBundle (
    BundleID       INT            NOT NULL IDENTITY(1,1),
    StudentID      INT            NOT NULL,
    TeacherID      INT            NOT NULL,
    LessonTypeID   INT            NOT NULL,
    AcademicYear   SMALLINT       NOT NULL,
    TotalLessons   INT            NOT NULL,
    PricePerLesson DECIMAL(10, 2) NOT NULL,
    StartDate      DATE           NOT NULL,
    EndDate        DATE           NOT NULL,
    QuarterSize    AS (TotalLessons / 4) PERSISTED,
    IsActive       BIT            NOT NULL DEFAULT 1,
    Notes          NVARCHAR(500)  NULL,
    CreatedAt      DATETIME2      NOT NULL DEFAULT SYSUTCDATETIME(),

    CONSTRAINT PK_LessonBundle PRIMARY KEY (BundleID),
    CONSTRAINT FK_Bundle_Student
        FOREIGN KEY (StudentID)    REFERENCES Student(StudentID),
    CONSTRAINT FK_Bundle_Teacher
        FOREIGN KEY (TeacherID)    REFERENCES Teacher(TeacherID),
    CONSTRAINT FK_Bundle_LessonType
        FOREIGN KEY (LessonTypeID) REFERENCES LessonType(LessonTypeID),
    CONSTRAINT CK_Bundle_TotalLessons
        CHECK (TotalLessons > 0 AND TotalLessons % 4 = 0),
    CONSTRAINT CK_Bundle_Price
        CHECK (PricePerLesson >= 0),
    CONSTRAINT CK_Bundle_Dates
        CHECK (EndDate > StartDate)
);


-- ------------------------------------------------------------
--  6. BundleQuarter
--     Each bundle splits into exactly 4 quarters.
--     LessonsAllocated = TotalLessons / 4.
--     LessonsUsed is incremented by the application as
--     lessons are completed or forfeited.
-- ------------------------------------------------------------
CREATE TABLE BundleQuarter (
    QuarterID        INT      NOT NULL IDENTITY(1,1),
    BundleID         INT      NOT NULL,
    QuarterNumber    TINYINT  NOT NULL,
    LessonsAllocated INT      NOT NULL,
    LessonsUsed      INT      NOT NULL DEFAULT 0,
    QuarterStartDate DATE     NOT NULL,
    QuarterEndDate   DATE     NOT NULL,

    CONSTRAINT PK_BundleQuarter PRIMARY KEY (QuarterID),
    CONSTRAINT FK_BundleQuarter_Bundle
        FOREIGN KEY (BundleID) REFERENCES LessonBundle(BundleID),
    CONSTRAINT UQ_BundleQuarter_Number
        UNIQUE (BundleID, QuarterNumber),
    CONSTRAINT CK_BundleQuarter_Number
        CHECK (QuarterNumber BETWEEN 1 AND 4),
    CONSTRAINT CK_BundleQuarter_Used
        CHECK (LessonsUsed >= 0 AND LessonsUsed <= LessonsAllocated)
);


-- ------------------------------------------------------------
--  7. ScheduledSlot
--     The recurring weekly pattern for a student/teacher pair.
--     EffectiveTo = NULL means the slot is still active.
--     When a slot changes, close it (set EffectiveTo) and
--     open a new one — preserving the history of past lessons.
-- ------------------------------------------------------------
CREATE TABLE ScheduledSlot (
    SlotID        INT     NOT NULL IDENTITY(1,1),
    StudentID     INT     NOT NULL,
    TeacherID     INT     NOT NULL,
    LessonTypeID  INT     NOT NULL,
    DayOfWeek     TINYINT NOT NULL,   -- 1 = Monday ... 7 = Sunday (ISO 8601)
    SlotTime      TIME(0) NOT NULL,
    EffectiveFrom DATE    NOT NULL,
    EffectiveTo   DATE    NULL,       -- NULL = ongoing
    IsActive      BIT     NOT NULL DEFAULT 1,

    CONSTRAINT PK_ScheduledSlot PRIMARY KEY (SlotID),
    CONSTRAINT FK_Slot_Student
        FOREIGN KEY (StudentID)    REFERENCES Student(StudentID),
    CONSTRAINT FK_Slot_Teacher
        FOREIGN KEY (TeacherID)    REFERENCES Teacher(TeacherID),
    CONSTRAINT FK_Slot_LessonType
        FOREIGN KEY (LessonTypeID) REFERENCES LessonType(LessonTypeID),
    CONSTRAINT CK_Slot_DayOfWeek
        CHECK (DayOfWeek BETWEEN 1 AND 7),
    CONSTRAINT CK_Slot_Dates
        CHECK (EffectiveTo IS NULL OR EffectiveTo > EffectiveFrom)
);


-- ------------------------------------------------------------
--  8. Lesson
--     One row per lesson instance, generated from a slot.
--     Draws a credit from a BundleQuarter.
--
--     Status values:
--       Scheduled        - upcoming, not yet attended
--       Completed        - attended; credit consumed
--       CancelledTeacher - teacher cancelled; credit NOT forfeited;
--                          teacher is responsible for rescheduling
--       Rescheduled      - moved lesson; OriginalLessonID links to
--                          the lesson that was cancelled
--       CancelledStudent - student cancelled; teacher decides outcome
--       Forfeited        - student-cancelled; teacher chose not to
--                          reschedule; credit forfeited
-- ------------------------------------------------------------
CREATE TABLE Lesson (
    LessonID           INT           NOT NULL IDENTITY(1,1),
    SlotID             INT           NOT NULL,
    BundleID           INT           NOT NULL,
    QuarterID          INT           NOT NULL,
    ScheduledDate      DATE          NOT NULL,
    ScheduledTime      TIME(0)       NOT NULL,
    Status             NVARCHAR(20)  NOT NULL DEFAULT 'Scheduled',
    CreditForfeited    BIT           NOT NULL DEFAULT 0,
    CancelledBy        NVARCHAR(10)  NULL,       -- 'Teacher' | 'Student'
    CancellationReason NVARCHAR(300) NULL,
    OriginalLessonID   INT           NULL,       -- populated on rescheduled lessons
    CompletedAt        DATETIME2     NULL,
    CreatedAt          DATETIME2     NOT NULL DEFAULT SYSUTCDATETIME(),

    CONSTRAINT PK_Lesson PRIMARY KEY (LessonID),
    CONSTRAINT FK_Lesson_Slot
        FOREIGN KEY (SlotID)           REFERENCES ScheduledSlot(SlotID),
    CONSTRAINT FK_Lesson_Bundle
        FOREIGN KEY (BundleID)         REFERENCES LessonBundle(BundleID),
    CONSTRAINT FK_Lesson_Quarter
        FOREIGN KEY (QuarterID)        REFERENCES BundleQuarter(QuarterID),
    CONSTRAINT FK_Lesson_Original
        FOREIGN KEY (OriginalLessonID) REFERENCES Lesson(LessonID),
    CONSTRAINT CK_Lesson_Status
        CHECK (Status IN (
            'Scheduled', 'Completed',
            'CancelledTeacher', 'Rescheduled',
            'CancelledStudent', 'Forfeited'
        )),
    CONSTRAINT CK_Lesson_CancelledBy
        CHECK (CancelledBy IN ('Teacher', 'Student') OR CancelledBy IS NULL)
);


-- ------------------------------------------------------------
--  9. ExtraLesson
--     Ad-hoc lessons purchased once a bundle is exhausted.
--     PriceCharged stores the agreed rate (base or overridden).
-- ------------------------------------------------------------
CREATE TABLE ExtraLesson (
    ExtraLessonID INT            NOT NULL IDENTITY(1,1),
    StudentID     INT            NOT NULL,
    TeacherID     INT            NOT NULL,
    LessonTypeID  INT            NOT NULL,
    ScheduledDate DATE           NOT NULL,
    ScheduledTime TIME(0)        NOT NULL,
    PriceCharged  DECIMAL(10, 2) NOT NULL,
    Status        NVARCHAR(20)   NOT NULL DEFAULT 'Scheduled',
    Notes         NVARCHAR(300)  NULL,
    CreatedAt     DATETIME2      NOT NULL DEFAULT SYSUTCDATETIME(),

    CONSTRAINT PK_ExtraLesson PRIMARY KEY (ExtraLessonID),
    CONSTRAINT FK_Extra_Student
        FOREIGN KEY (StudentID)    REFERENCES Student(StudentID),
    CONSTRAINT FK_Extra_Teacher
        FOREIGN KEY (TeacherID)    REFERENCES Teacher(TeacherID),
    CONSTRAINT FK_Extra_LessonType
        FOREIGN KEY (LessonTypeID) REFERENCES LessonType(LessonTypeID),
    CONSTRAINT CK_Extra_Price
        CHECK (PriceCharged >= 0),
    CONSTRAINT CK_Extra_Status
        CHECK (Status IN ('Scheduled', 'Completed', 'Cancelled', 'Forfeited'))
);


-- ------------------------------------------------------------
-- 10. Invoice
--     A bundle is invoiced in 12 equal monthly instalments.
--     Amount per row = (TotalLessons * PricePerLesson) / 12,
--     calculated and written by the application layer.
-- ------------------------------------------------------------
CREATE TABLE Invoice (
    InvoiceID           INT            NOT NULL IDENTITY(1,1),
    BundleID            INT            NOT NULL,
    AccountHolderID     INT            NOT NULL,
    InstallmentNumber   TINYINT        NOT NULL,   -- 1-12
    Amount              DECIMAL(10, 2) NOT NULL,
    DueDate             DATE           NOT NULL,
    PaidDate            DATE           NULL,
    Status              NVARCHAR(20)   NOT NULL DEFAULT 'Pending',
    Notes               NVARCHAR(300)  NULL,
    CreatedAt           DATETIME2      NOT NULL DEFAULT SYSUTCDATETIME(),

    CONSTRAINT PK_Invoice PRIMARY KEY (InvoiceID),
    CONSTRAINT FK_Invoice_Bundle
        FOREIGN KEY (BundleID)        REFERENCES LessonBundle(BundleID),
    CONSTRAINT FK_Invoice_AccountHolder
        FOREIGN KEY (AccountHolderID) REFERENCES AccountHolder(AccountHolderID),
    CONSTRAINT UQ_Invoice_Installment
        UNIQUE (BundleID, InstallmentNumber),
    CONSTRAINT CK_Invoice_InstallmentNumber
        CHECK (InstallmentNumber BETWEEN 1 AND 12),
    CONSTRAINT CK_Invoice_Amount
        CHECK (Amount >= 0),
    CONSTRAINT CK_Invoice_Status
        CHECK (Status IN ('Pending', 'Paid', 'Overdue', 'Void'))
);


-- ============================================================
--  INDEXES
-- ============================================================

CREATE INDEX IX_AccountHolder_Teacher ON AccountHolder (TeacherID);
CREATE INDEX IX_Student_AccountHolder ON Student       (AccountHolderID);
CREATE INDEX IX_Bundle_Student        ON LessonBundle  (StudentID);
CREATE INDEX IX_Bundle_Teacher        ON LessonBundle  (TeacherID);
CREATE INDEX IX_Bundle_Year           ON LessonBundle  (AcademicYear);
CREATE INDEX IX_Quarter_Bundle        ON BundleQuarter (BundleID);
CREATE INDEX IX_Slot_Student          ON ScheduledSlot (StudentID, IsActive);
CREATE INDEX IX_Slot_Teacher          ON ScheduledSlot (TeacherID, IsActive);
CREATE INDEX IX_Lesson_Bundle         ON Lesson        (BundleID);
CREATE INDEX IX_Lesson_Date           ON Lesson        (ScheduledDate);
CREATE INDEX IX_Lesson_Status         ON Lesson        (Status);
CREATE INDEX IX_ExtraLesson_Student   ON ExtraLesson   (StudentID);
CREATE INDEX IX_Invoice_Bundle        ON Invoice       (BundleID);
CREATE INDEX IX_Invoice_AccountHolder ON Invoice       (AccountHolderID, Status);
CREATE INDEX IX_Invoice_DueDate       ON Invoice       (DueDate, Status);
