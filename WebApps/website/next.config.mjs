// next.config.mjs
import path from "path";
import { fileURLToPath } from "url";

// محاسبه __dirname در ES Modules
const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);

/** @type {import('next').NextConfig} */
const nextConfig = {
  webpack: (config) => {
    config.resolve.alias = {
      ...config.resolve.alias,
      "@": path.resolve(__dirname),
      "@components": path.resolve(__dirname, "components"), 
      "@public": path.resolve(__dirname, "public"),
      '@lib': path.resolve(__dirname, 'lib'),
      '@constants': path.resolve(__dirname, 'constants'),
      '@hooks': path.resolve(__dirname, 'hooks'),
    };
    return config;
  },
};

export default nextConfig;
