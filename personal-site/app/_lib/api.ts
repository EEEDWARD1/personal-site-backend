const API_BASE_URL =
  process.env.NEXT_PUBLIC_API_BASE_URL ?? "https://wbapi.eduardteodor.co.uk";

export type ApiState<T> =
  | { ok: true; data: T }
  | { ok: false; message: string; status?: number };

export type BlogPost = {
  id?: number;
  slug: string;
  title: string;
  content?: string;
  excerpt?: string;
  starred?: boolean;
  published?: boolean;
  publishedAt?: string;
  createdAt?: string;
};

export type Project = {
  id?: number;
  slug: string;
  title: string;
  summary?: string;
  description?: string;
  techStack?: string;
  githubUrl?: string;
  liveUrl?: string;
  status?: string;
  featured?: boolean;
  published?: boolean;
  displayOrder?: number;
  createdAt?: string;
};

export type FreelanceProject = {
  id?: number;
  slug: string;
  clientName?: string;
  projectTitle: string;
  summary?: string;
  description?: string;
  services?: string[];
  testimonial?: string;
  websiteUrl?: string;
  thumbnailUrl?: string;
  featured?: boolean;
  published?: boolean;
  completedAt?: string;
  displayOrder?: number;
  createdAt?: string;
};

export type AuthStatus =
  | "AUTHENTICATED"
  | "TOTP_SETUP_REQUIRED"
  | "TOTP_REQUIRED";

export type AuthResponse = {
  status: AuthStatus;
  token: string | null;
  preAuthToken: string | null;
  secret: string | null;
  qrCode: string | null;
};

export function getApiUrl(path: string) {
  return `${API_BASE_URL}${path}`;
}

export async function apiFetch<T>(
  path: string,
  init?: RequestInit,
): Promise<ApiState<T>> {
  try {
    const response = await fetch(getApiUrl(path), {
      ...init,
      headers: {
        "Content-Type": "application/json",
        ...init?.headers,
      },
      cache: "no-store",
    });

    if (!response.ok) {
      return {
        ok: false,
        status: response.status,
        message: readableStatus(response.status),
      };
    }

    if (response.status === 204) {
      return { ok: true, data: undefined as T };
    }

    return { ok: true, data: (await response.json()) as T };
  } catch {
    return {
      ok: false,
      message:
        "The content service is not available right now. Please try again shortly.",
    };
  }
}

export function authHeaders(token: string): HeadersInit {
  return { Authorization: `Bearer ${token}` };
}

export const publicApi = {
  posts: (starredOnly = false) =>
    apiFetch<BlogPost[]>(
      `/api/blog/posts${starredOnly ? "?starredOnly=true" : ""}`,
    ),
  post: (slug: string) => apiFetch<BlogPost>(`/api/blog/posts/${slug}`),
  projects: (featuredOnly = false) =>
    apiFetch<Project[]>(
      `/api/projects${featuredOnly ? "?featuredOnly=true" : ""}`,
    ),
  project: (slug: string) => apiFetch<Project>(`/api/projects/${slug}`),
  freelance: (featuredOnly = false) =>
    apiFetch<FreelanceProject[]>(
      `/api/freelance${featuredOnly ? "?featuredOnly=true" : ""}`,
    ),
  freelanceProject: (slug: string) =>
    apiFetch<FreelanceProject>(`/api/freelance/${slug}`),
};

function readableStatus(status: number) {
  switch (status) {
    case 401:
      return "Please check your details and try again.";
    case 403:
      return "You do not have access to this area.";
    case 404:
      return "That item could not be found.";
    case 409:
      return "That slug is already in use.";
    default:
      return "Something went wrong while loading this content.";
  }
}
