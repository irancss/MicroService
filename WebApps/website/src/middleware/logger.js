import { NextResponse } from 'next/server';

export function logger(request) {
  const { pathname, search } = request.nextUrl;
  const method = request.method || 'GET';
  const timestamp = new Date().toISOString();
  
  console.log(`[${timestamp}] [MIDDLEWARE] ${method} ${pathname}${search || ''}`);
  
  // You can add more logging details here as needed
  if (request.cookies) {
    console.log(`[MIDDLEWARE] Cookies: ${JSON.stringify([...request.cookies.entries()])}`);
  }
  
  // If this is being used in middleware chain
  return NextResponse.next();
}

// Optional config to specify which routes to run on
export const config = {
  matcher: '/((?!api|_next/static|_next/image|favicon.ico).*)',
};