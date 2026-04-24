// ===== API Response =====
export interface ApiResponse<T> {
  success: boolean;
  data: T;
  message?: string;
  errors?: string[];
  meta?: PaginationMeta;
}

export interface PaginationMeta {
  currentPage: number;
  totalPages: number;
  pageSize: number;
  totalCount: number;
  hasPrevious: boolean;
  hasNext: boolean;
}

// ===== Auth =====
export interface LoginRequest { userName: string; password: string; }
export interface RegisterRequest {
  firstName: string; lastName: string; userName: string; email?: string;
  password: string; confirmPassword: string; role: string; gradeId?: number; preferredLanguage: string;
}
export interface StudentPinLoginRequest { studentCode: string; pin: string; }
export interface GuestCreateRequest { displayName: string; gradeId: number; }
export interface AuthResponse {
  userId: string; userName: string; firstName: string; lastName: string;
  role: string; gradeId?: number; avatarUrl?: string;
  accessToken: string; refreshToken: string; accessTokenExpiry: Date; preferredLanguage: string;
}
export interface GuestSession {
  sessionId: string; displayName: string; gradeId: number; sessionToken: string; expiresAt: Date;
}
export interface UserProfile {
  id: string; userName: string; firstName: string; lastName: string;
  email?: string; role: string; gradeId?: number; gradeName?: string;
  avatarUrl?: string; studentCode?: string; preferredLanguage: string;
  createdAt: Date; lastLoginAt?: Date;
}

// ===== Content =====
export interface Grade {
  id: number; nameAr: string; nameEn: string; level: number;
  schoolType: string; displayOrder: number; isActive: boolean; unitsCount: number;
}
export interface Unit {
  id: number; gradeId: number; nameAr: string; nameEn: string;
  unitNumber: number; isActive: boolean; lessonsCount: number;
}
export interface Lesson {
  id: number; unitId: number; nameAr: string; nameEn: string;
  lessonNumber: number; isActive: boolean;
}

// ===== Questions =====
export interface Question {
  id: number; questionText: string; instructionAr?: string;
  questionType: string; difficultyLevel: string; skillCategory: string; contentTopic?: string;
  gradeId: number; testType?: string; imageUrl?: string; audioUrl?: string; videoUrl?: string;
  passageText?: string; correctAnswer?: string; explanation?: string; hintText?: string;
  tags?: string; points: number; estimatedTimeMinutes?: number;
  options: QuestionOption[]; matchingPairs: MatchingPair[]; subQuestions: SubQuestion[];
  createdAt: Date;
}
export interface QuestionOption { id: number; optionText: string; isCorrect: boolean; orderIndex: number; imageUrl?: string; }
export interface MatchingPair { id: number; leftText: string; rightText: string; leftImageUrl?: string; rightImageUrl?: string; pairIndex: number; }
export interface SubQuestion { id: number; orderIndex: number; text: string; questionType: string; options?: string; correctAnswer: string; explanation?: string; points: number; }

// ===== Tests =====
export interface Test {
  id: number; titleAr: string; titleEn: string; descriptionAr?: string; descriptionEn?: string;
  instructions?: string; gradeId: number; gradeName?: string; unitId?: number; unitName?: string;
  lessonId?: number; lessonName?: string; testType: string; skillCategory?: string;
  isTimedTest: boolean; timeLimitMinutes?: number; passingScore: number; totalPoints: number;
  questionCount: number; isPublished: boolean; allowRetake: boolean; maxRetakeCount?: number; createdAt: Date;
}
export interface TestDetail extends Test {
  shuffleQuestions: boolean; shuffleOptions: boolean; showCorrectAnswers: boolean;
  showExplanations: boolean; availableFrom?: Date; availableTo?: Date; questions: Question[];
}
export interface AttemptStart {
  attemptId: number; testId: number; testTitle: string; timeLimitMinutes?: number;
  isTimedTest: boolean; startedAt: Date; questions: Question[];
}
export interface SubmitAttempt {
  attemptId: number; testId: number; answers: AnswerSubmission[];
}
export interface AnswerSubmission { questionId: number; answerText?: string; selectedOptionIds?: number[]; timeSpentSeconds: number; }
export interface AttemptResult {
  attemptId: number; testId: number; testTitle: string; score: number; maxScore: number;
  percentage: number; correctAnswers: number; wrongAnswers: number; skippedAnswers: number;
  timeSpentSeconds: number; passed: boolean; attemptNumber: number; pointsEarned: number;
  answerDetails: AnswerResult[]; badgesEarned: string[];
}
export interface AnswerResult {
  questionId: number; questionText: string; selectedAnswer?: string; correctAnswer?: string;
  isCorrect: boolean; pointsEarned: number; explanation?: string; timeSpentSeconds: number;
}
export interface AttemptSummary {
  attemptId: number; testId: number; testTitle: string; percentage: number;
  passed: boolean; attemptNumber: number; timeSpentSeconds: number; submittedAt: Date;
}

// ===== Games =====
export interface MatchingGame {
  id: number; gameTitle: string; instructions?: string; gradeId: number; skillCategory: string;
  numberOfPairs: number; matchingMode: string; timerMode: string; timeLimitSeconds?: number;
  difficultyLevel: string; pointsPerMatch: number; enableHints: boolean; maxHints: number;
  thumbnailUrl?: string; category?: string; pairs: MatchingGamePair[];
}
export interface MatchingGamePair {
  id: number; questionText: string; questionImageUrl?: string; questionAudioUrl?: string;
  answerText: string; answerImageUrl?: string; answerAudioUrl?: string; explanation?: string; pairOrder: number;
}
export interface MatchingGameStart {
  gameId: number; sessionId: number; gameTitle: string; matchingMode: string; timerMode: string;
  timeLimitSeconds?: number; pointsPerMatch: number; enableHints: boolean; maxHints: number;
  leftItems: ShuffledItem[]; rightItems: ShuffledItem[];
  pairs?: MatchingGamePair[];
}
export interface ShuffledItem { pairId: number; text: string; imageUrl?: string; audioUrl?: string; }
export interface GameSessionResult {
  sessionId: number; gameId: number; gameType: string; totalScore: number;
  correctCount: number; wrongCount: number; timeSpentSeconds: number; hintsUsed: number;
  isCompleted: boolean; pointsEarned: number; badgesEarned: string[];
}
