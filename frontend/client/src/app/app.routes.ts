import { Routes } from '@angular/router';

export const routes: Routes = [
    {
        path: '',
        loadComponent: () => import('./home/home.component')
    },
    {
        path: 'upload',
        loadComponent: () => import('./upload/upload.component')
    },
    {
        path: 'logout',
        loadComponent: () => import('./user/logout/logout.component')
    }
];
