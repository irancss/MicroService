const API_BASE_URL = '/api'; // یا از env بگیر

export const apiRoutes = {
  auth: {
    login: `${API_BASE_URL}/auth/login`,
    logout: `${API_BASE_URL}/auth/logout`,
    register: `${API_BASE_URL}/auth/register`,
  },
  user: {
    getProfile: `${API_BASE_URL}/user/profile`,
    updateProfile: `${API_BASE_URL}/user/update`,
    list: `${API_BASE_URL}/user/list`,
  },
  products: {
    list: `${API_BASE_URL}/products`,
    detail: (id) => `${API_BASE_URL}/products/${id}`,
    create: `${API_BASE_URL}/products/create`,
  },
};
