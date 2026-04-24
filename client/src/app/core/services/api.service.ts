import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse, Grade, Unit, Lesson, Question, Test, TestDetail, AttemptStart, AttemptResult, AttemptSummary, SubmitAttempt, MatchingGame, MatchingGameStart, GameSessionResult } from '../models';

@Injectable({ providedIn: 'root' })
export class ApiService {
  private readonly base = environment.apiUrl;

  constructor(private http: HttpClient) {}

  // ===== Grades =====
  getGrades(): Observable<ApiResponse<Grade[]>> {
    return this.http.get<ApiResponse<Grade[]>>(`${this.base}/grades`);
  }
  getGrade(id: number): Observable<ApiResponse<Grade>> {
    return this.http.get<ApiResponse<Grade>>(`${this.base}/grades/${id}`);
  }
  getUnits(gradeId: number): Observable<ApiResponse<Unit[]>> {
    return this.http.get<ApiResponse<Unit[]>>(`${this.base}/grades/${gradeId}/units`);
  }
  getLessons(gradeId: number, unitId: number): Observable<ApiResponse<Lesson[]>> {
    return this.http.get<ApiResponse<Lesson[]>>(`${this.base}/grades/${gradeId}/units/${unitId}/lessons`);
  }

  // ===== Questions (Admin) =====
  getQuestions(filters: any = {}): Observable<ApiResponse<Question[]>> {
    let params = new HttpParams();
    Object.keys(filters).forEach(k => { if (filters[k] != null) params = params.set(k, filters[k]); });
    return this.http.get<ApiResponse<Question[]>>(`${this.base}/questions`, { params });
  }
  getQuestion(id: number): Observable<ApiResponse<Question>> {
    return this.http.get<ApiResponse<Question>>(`${this.base}/questions/${id}`);
  }
  createQuestion(data: any): Observable<ApiResponse<Question>> {
    return this.http.post<ApiResponse<Question>>(`${this.base}/questions`, data);
  }
  updateQuestion(id: number, data: any): Observable<ApiResponse<Question>> {
    return this.http.put<ApiResponse<Question>>(`${this.base}/questions/${id}`, data);
  }
  deleteQuestion(id: number): Observable<ApiResponse<string>> {
    return this.http.delete<ApiResponse<string>>(`${this.base}/questions/${id}`);
  }

  // ===== Tests =====
  getTests(filters: any = {}): Observable<ApiResponse<Test[]>> {
    let params = new HttpParams();
    Object.keys(filters).forEach(k => { if (filters[k] != null) params = params.set(k, filters[k]); });
    return this.http.get<ApiResponse<Test[]>>(`${this.base}/tests`, { params });
  }
  getTest(id: number): Observable<ApiResponse<TestDetail>> {
    return this.http.get<ApiResponse<TestDetail>>(`${this.base}/tests/${id}`);
  }
  createTest(data: any): Observable<ApiResponse<Test>> {
    return this.http.post<ApiResponse<Test>>(`${this.base}/tests`, data);
  }
  updateTest(id: number, data: any): Observable<ApiResponse<Test>> {
    return this.http.put<ApiResponse<Test>>(`${this.base}/tests/${id}`, data);
  }
  deleteTest(id: number): Observable<ApiResponse<string>> {
    return this.http.delete<ApiResponse<string>>(`${this.base}/tests/${id}`);
  }
  publishTest(id: number, publish: boolean): Observable<ApiResponse<string>> {
    return this.http.post<ApiResponse<string>>(`${this.base}/tests/${id}/publish?publish=${publish}`, {});
  }
  addQuestionsToTest(testId: number, questionIds: number[]): Observable<ApiResponse<string>> {
    return this.http.post<ApiResponse<string>>(`${this.base}/tests/${testId}/questions`, { questionIds });
  }
  removeQuestionsFromTest(testId: number, questionIds: number[]): Observable<ApiResponse<string>> {
    return this.http.delete<ApiResponse<string>>(`${this.base}/tests/${testId}/questions`, { body: { questionIds } });
  }
  startTest(testId: number): Observable<ApiResponse<AttemptStart>> {
    return this.http.post<ApiResponse<AttemptStart>>(`${this.base}/tests/${testId}/start`, {});
  }
  submitTest(testId: number, data: SubmitAttempt): Observable<ApiResponse<AttemptResult>> {
    return this.http.post<ApiResponse<AttemptResult>>(`${this.base}/tests/${testId}/submit`, data);
  }
  getTestResults(testId: number): Observable<ApiResponse<AttemptSummary[]>> {
    return this.http.get<ApiResponse<AttemptSummary[]>>(`${this.base}/tests/${testId}/results`);
  }
  getAttemptDetail(attemptId: number): Observable<ApiResponse<AttemptResult>> {
    return this.http.get<ApiResponse<AttemptResult>>(`${this.base}/tests/attempts/${attemptId}`);
  }

  // ===== Games: Matching =====
  getMatchingGames(filters: any = {}): Observable<ApiResponse<MatchingGame[]>> {
    let params = new HttpParams();
    Object.keys(filters).forEach(k => { if (filters[k] != null) params = params.set(k, filters[k]); });
    return this.http.get<ApiResponse<MatchingGame[]>>(`${this.base}/games/matching`, { params });
  }
  createMatchingGame(data: any): Observable<ApiResponse<MatchingGame>> {
    return this.http.post<ApiResponse<MatchingGame>>(`${this.base}/games/matching`, data);
  }
  deleteMatchingGame(id: number): Observable<ApiResponse<string>> {
    return this.http.delete<ApiResponse<string>>(`${this.base}/games/matching/${id}`);
  }
  startMatchingGame(gameId: number): Observable<ApiResponse<MatchingGameStart>> {
    return this.http.post<ApiResponse<MatchingGameStart>>(`${this.base}/games/matching/${gameId}/start`, {});
  }
  submitMatchingGame(data: any): Observable<ApiResponse<GameSessionResult>> {
    return this.http.post<ApiResponse<GameSessionResult>>(`${this.base}/games/matching/submit`, data);
  }

  // ===== Games: Wheel =====
  startWheelGame(gradeId: number): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/games/wheel/${gradeId}/start`, {});
  }
  spinWheel(sessionId: number): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/games/wheel/${sessionId}/spin`, {});
  }
  answerWheel(data: any): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/games/wheel/answer`, data);
  }
  endWheelGame(sessionId: number): Observable<ApiResponse<GameSessionResult>> {
    return this.http.post<ApiResponse<GameSessionResult>>(`${this.base}/games/wheel/${sessionId}/end`, {});
  }

  // ===== Games: DragDrop =====
  getDragDropGames(filters: any = {}): Observable<ApiResponse<any[]>> {
    let params = new HttpParams();
    Object.keys(filters).forEach(k => { if (filters[k] != null) params = params.set(k, filters[k]); });
    return this.http.get<ApiResponse<any[]>>(`${this.base}/games/dragdrop`, { params });
  }
  createDragDropGame(data: any): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/games/dragdrop`, data);
  }
  deleteDragDropGame(id: number): Observable<ApiResponse<string>> {
    return this.http.delete<ApiResponse<string>>(`${this.base}/games/dragdrop/${id}`);
  }
  startDragDropGame(gameId: number): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/games/dragdrop/${gameId}/start`, {});
  }
  submitDragDropGame(data: any): Observable<ApiResponse<GameSessionResult>> {
    return this.http.post<ApiResponse<GameSessionResult>>(`${this.base}/games/dragdrop/submit`, data);
  }

  // ===== Games: FlipCard =====
  getFlipCardGames(filters: any = {}): Observable<ApiResponse<any[]>> {
    let params = new HttpParams();
    Object.keys(filters).forEach(k => { if (filters[k] != null) params = params.set(k, filters[k]); });
    return this.http.get<ApiResponse<any[]>>(`${this.base}/games/flipcard`, { params });
  }
  createFlipCardGame(data: any): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/games/flipcard`, data);
  }
  deleteFlipCardGame(id: number): Observable<ApiResponse<string>> {
    return this.http.delete<ApiResponse<string>>(`${this.base}/games/flipcard/${id}`);
  }
  startFlipCardGame(gameId: number): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.base}/games/flipcard/${gameId}/start`, {});
  }
  submitFlipCardGame(data: any): Observable<ApiResponse<GameSessionResult>> {
    return this.http.post<ApiResponse<GameSessionResult>>(`${this.base}/games/flipcard/submit`, data);
  }

  // ===== Contact =====
  sendContactMessage(data: any): Observable<ApiResponse<string>> {
    return this.http.post<ApiResponse<string>>(`${this.base}/contact/message`, data);
  }
  sendBookingRequest(data: any): Observable<ApiResponse<string>> {
    return this.http.post<ApiResponse<string>>(`${this.base}/contact/booking`, data);
  }
}
