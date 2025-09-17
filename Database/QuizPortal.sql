CREATE TABLE Users (
    UserID INT PRIMARY KEY IDENTITY,
    Username NVARCHAR(50) UNIQUE NOT NULL,
    Password NVARCHAR(100) NOT NULL,
    FullName NVARCHAR(100) NOT NULL,
    Role NVARCHAR(20) NOT NULL CHECK (Role IN ('Teacher', 'Student', 'Administrator'))
);

CREATE TABLE Quizzes (
    QuizID INT PRIMARY KEY IDENTITY,
    Title NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500),
    CreatedBy INT FOREIGN KEY REFERENCES Users(UserID),
    CreatedDate DATETIME DEFAULT GETDATE(),
    IsActive BIT DEFAULT 1
);

CREATE TABLE Questions (
    QuestionID INT PRIMARY KEY IDENTITY,
    QuizID INT FOREIGN KEY REFERENCES Quizzes(QuizID),
    QuestionText NVARCHAR(500) NOT NULL,
    Option1 NVARCHAR(200) NOT NULL,
    Option2 NVARCHAR(200) NOT NULL,
    Option3 NVARCHAR(200),
    Option4 NVARCHAR(200),
    CorrectAnswer INT NOT NULL,
    DisplayOrder INT DEFAULT 0
);

CREATE TABLE StudentAnswers (
    AnswerID INT PRIMARY KEY IDENTITY,
    StudentID INT FOREIGN KEY REFERENCES Users(UserID),
    QuestionID INT FOREIGN KEY REFERENCES Questions(QuestionID),
    SelectedAnswer INT NOT NULL,
    AnswerDate DATETIME DEFAULT GETDATE()
);

CREATE TABLE QuizResults (
    ResultID INT PRIMARY KEY IDENTITY,
    StudentID INT FOREIGN KEY REFERENCES Users(UserID),
    QuizID INT FOREIGN KEY REFERENCES Quizzes(QuizID),
    Score INT NOT NULL,
    TotalQuestions INT NOT NULL,
    CompletionDate DATETIME DEFAULT GETDATE()
);
CREATE TABLE StudentQuizAttempts (
    AttemptID INT IDENTITY(1,1) PRIMARY KEY,
    StudentID INT NOT NULL,
    QuizID INT NOT NULL,
    AttemptDate DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_StudentQuiz FOREIGN KEY (StudentID) REFERENCES Users(UserID),
    CONSTRAINT FK_Quiz FOREIGN KEY (QuizID) REFERENCES Quizzes(QuizID),
    CONSTRAINT UQ_StudentQuiz UNIQUE (StudentID, QuizID)  -- Prevent multiple attempts
);
