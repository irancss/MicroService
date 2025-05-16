import { NextResponse } from 'next/server';
import { jwtVerify } from 'jose'; // Install jose package for JWT verification

/**
 * Authentication middleware for Next.js
 * @param {Request} request - The incoming request object
 * @returns {NextResponse} - The response object
 */
export async function authGuard(request) {
  // Define public paths that don't require authentication
  const publicPaths = ['/login', '/register', '/forgot-password', '/api/public'];
  
  // Get the path from URL
  const path = new URL(request.url).pathname;
  
  // Allow access to public paths without authentication
  if (publicPaths.some(publicPath => path.startsWith(publicPath))) {
    return NextResponse.next();
  }
  
  try {
    // Check for auth token in cookies
    const token = request.cookies.get('authToken')?.value;
    
    if (!token) {
      return NextResponse.redirect(new URL('/login', request.url));
    }
    
    // Verify JWT token (use your actual secret key)
    const secret = new TextEncoder().encode(process.env.JWT_SECRET || 'fallback_secret_key');
    const { payload } = await jwtVerify(token, secret);
    
    // Check if token is expired
    if (payload.exp && payload.exp * 1000 < Date.now()) {
      return NextResponse.redirect(new URL('/login?expired=true', request.url));
    }
    
    // Add user info to headers for downstream use
    const requestHeaders = new Headers(request.headers);
    requestHeaders.set('x-user-id', payload.sub);
    requestHeaders.set('x-user-role', payload.role || 'user');
    
    return NextResponse.next({
      request: {
        headers: requestHeaders,
      },
    });
  } catch (error) {
    console.error('Auth middleware error:', error);
    return NextResponse.redirect(new URL('/login?error=auth', request.url));
  }
}
