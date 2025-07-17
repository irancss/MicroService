import { NextResponse } from 'next/server';

export function logger(request) {
  const { pathname, search } = request.nextUrl;
  const method = request.method || 'GET';
  const timestamp = new Date().toISOString();
  
  
  // You can add more logging details here as needed
  if (request.cookies) {
  }
  
  // If this is being used in middleware chain
  return NextResponse.next();
}

// Optional config to specify which routes to run on
export const config = {
  matcher: '/((?!api|_next/static|_next/image|favicon.ico).*)',
};