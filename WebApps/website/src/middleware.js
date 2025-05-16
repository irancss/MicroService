// middleware.js
import { NextResponse } from 'next/server';
import { authGuard } from './middleware/auth';
import { logger } from './middleware/logger';
import { device } from './middleware/device';

export function middleware(request) {
//   logger(request);

//   // دستگاه رو تشخیص بده
//   const deviceResponse = device(request);
  
//   // گارد احراز هویت برای پنل
//   if (request.nextUrl.pathname.startsWith('/panel')) {
//     const authResponse = authGuard(request);
//     if (authResponse) return authResponse;
//   }

//   // If deviceResponse exists, return it, otherwise continue with the original request
//   return deviceResponse || NextResponse.next();
 }

export const config = {
  matcher: ['/((?!_next/static|_next/image|favicon.ico).*)'],
};
