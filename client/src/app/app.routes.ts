import { Routes } from '@angular/router';
import { authGuard, adminGuard, teacherGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () => import('./pages/home/home.component').then(m => m.HomeComponent)
  },
  {
    path: 'auth',
    children: [
      { path: 'login', loadComponent: () => import('./pages/auth/login/login.component').then(m => m.LoginComponent) },
      { path: 'register', loadComponent: () => import('./pages/auth/register/register.component').then(m => m.RegisterComponent) },
    ]
  },
  {
    path: 'grades',
    loadComponent: () => import('./pages/grades/grades.component').then(m => m.GradesComponent)
  },
  {
    path: 'tests',
    children: [
      { path: '', loadComponent: () => import('./pages/tests/test-list/test-list.component').then(m => m.TestListComponent) },
      { path: ':id', loadComponent: () => import('./pages/tests/test-detail/test-detail.component').then(m => m.TestDetailComponent) },
      { path: ':id/take', loadComponent: () => import('./pages/tests/test-take/test-take.component').then(m => m.TestTakeComponent), canActivate: [authGuard] },
      { path: ':id/result/:attemptId', loadComponent: () => import('./pages/tests/test-result/test-result.component').then(m => m.TestResultComponent), canActivate: [authGuard] },
    ]
  },
  {
    path: 'games',
    children: [
      { path: '', loadComponent: () => import('./pages/games/game-hub/game-hub.component').then(m => m.GameHubComponent) },
      { path: 'matching/:id', loadComponent: () => import('./pages/games/matching/matching.component').then(m => m.MatchingComponent) },
      { path: 'wheel', loadComponent: () => import('./pages/games/wheel/wheel.component').then(m => m.WheelComponent) },
      { path: 'dragdrop/:id', loadComponent: () => import('./pages/games/dragdrop/dragdrop.component').then(m => m.DragdropComponent) },
      { path: 'flipcard/:id', loadComponent: () => import('./pages/games/flipcard/flipcard.component').then(m => m.FlipcardComponent) },
    ]
  },
  {
    path: 'contact',
    loadComponent: () => import('./pages/contact/contact.component').then(m => m.ContactComponent)
  },
  {
    path: 'booking',
    loadComponent: () => import('./pages/booking/booking.component').then(m => m.BookingComponent)
  },
  {
    path: 'admin',
    canActivate: [teacherGuard],
    children: [
      { path: '', loadComponent: () => import('./pages/admin/dashboard/dashboard.component').then(m => m.DashboardComponent) },
      { path: 'questions', loadComponent: () => import('./pages/admin/questions/questions.component').then(m => m.QuestionsComponent) },
      { path: 'tests', loadComponent: () => import('./pages/admin/tests/tests.component').then(m => m.AdminTestsComponent) },
      { path: 'games', loadComponent: () => import('./pages/admin/games/games.component').then(m => m.AdminGamesComponent) },
    ]
  },
  { path: '**', redirectTo: '' }
];
