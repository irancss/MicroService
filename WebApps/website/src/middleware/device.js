import { NextResponse } from 'next/server';

export function middleware(request) {
  const userAgent = request.headers.get('user-agent') || '';
  
  // More comprehensive device detection
  const isMobile = /android|iphone|ipod|webos|iemobile|opera mini/i.test(userAgent);
  const isTablet = /ipad|android(?!.*mobile)/i.test(userAgent);
  const isDesktop = !isMobile && !isTablet;
  
  const deviceType = isMobile ? 'mobile' : isTablet ? 'tablet' : 'desktop';
  
  const response = NextResponse.next();
  
  // Set cookies with options
  response.cookies.set('is-mobile', isMobile ? '1' : '0', {
    maxAge: 60 * 60 * 24, // 1 day
    path: '/',
  });
  
  response.cookies.set('device-type', deviceType, {
    maxAge: 60 * 60 * 24,
    path: '/',
  });
  
  return response;
}

// Configure which paths this middleware runs on
export const config = {
  matcher: ['/:path*'],
};
