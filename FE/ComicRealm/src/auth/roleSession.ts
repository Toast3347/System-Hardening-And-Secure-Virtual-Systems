import { UserRole } from '../models/enums/UserRole'

const ACCESS_TOKEN_STORAGE_KEY = 'comicrealm.accessToken'

export const roleLabels: Record<UserRole, string> = {
  [UserRole.SuperAdmin]: 'SuperAdmin',
  [UserRole.Admin]: 'Admin',
  [UserRole.Friend]: 'Friend',
}

type JwtPayload = Record<string, unknown>

function decodeJwtPayload(token: string): JwtPayload | null {
  const parts = token.split('.')
  if (parts.length !== 3) {
    return null
  }

  const payloadPart = parts[1]
  if (!payloadPart) {
    return null
  }

  try {
    const normalized = payloadPart.replace(/-/g, '+').replace(/_/g, '/')
    const decoded = atob(normalized)
    return JSON.parse(decoded) as JwtPayload
  } catch {
    return null
  }
}

function roleFromText(roleText: string): UserRole | null {
  const normalized = roleText.trim().toLowerCase()

  if (normalized === 'superadmin' || normalized === 'super admin') {
    return UserRole.SuperAdmin
  }

  if (normalized === 'admin') {
    return UserRole.Admin
  }

  if (normalized === 'friend') {
    return UserRole.Friend
  }

  return null
}

function getStringClaim(payload: JwtPayload, keys: string[]): string | null {
  for (const key of keys) {
    const value = payload[key]
    if (typeof value === 'string' && value.trim().length > 0) {
      return value
    }
  }

  return null
}

function getNumberClaim(payload: JwtPayload, keys: string[]): number | null {
  for (const key of keys) {
    const value = payload[key]
    if (typeof value === 'number' && Number.isFinite(value)) {
      return value
    }

    if (typeof value === 'string') {
      const parsed = Number.parseInt(value, 10)
      if (!Number.isNaN(parsed)) {
        return parsed
      }
    }
  }

  return null
}

export function getAccessToken(): string | null {
  return localStorage.getItem(ACCESS_TOKEN_STORAGE_KEY)
}

export function setAuthSession(token: string): void {
  localStorage.setItem(ACCESS_TOKEN_STORAGE_KEY, token)
}

export function getCurrentUserRole(): UserRole | null {
  const token = getAccessToken()

  if (!token) {
    return null
  }

  const payload = decodeJwtPayload(token)

  if (!payload) {
    return null
  }

  const roleClaim = getStringClaim(payload, [
    'role',
    'http://schemas.microsoft.com/ws/2008/06/identity/claims/role',
  ])

  if (!roleClaim) {
    return null
  }

  const parsedRole = roleFromText(roleClaim)

  if (parsedRole === null) {
    return null
  }

  return parsedRole
}

export function getCurrentUserId(): number | null {
  const token = getAccessToken()
  if (!token) {
    return null
  }

  const payload = decodeJwtPayload(token)
  if (!payload) {
    return null
  }

  return getNumberClaim(payload, [
    'nameid',
    'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier',
  ])
}

export function clearCurrentUserRole(): void {
  localStorage.removeItem(ACCESS_TOKEN_STORAGE_KEY)
}

export function isAuthenticated(role: UserRole | null): boolean {
  return role !== null && !isSessionExpired()
}

export function isSessionExpired(): boolean {
  const token = getAccessToken()
  if (!token) {
    return true
  }

  const payload = decodeJwtPayload(token)
  if (!payload) {
    return true
  }

  const expiry = getNumberClaim(payload, ['exp'])
  if (!expiry) {
    return false
  }

  const nowInSeconds = Math.floor(Date.now() / 1000)
  return expiry <= nowInSeconds
}

export function getCreatableRolesByActor(actorRole: UserRole | null): UserRole[] {
  if (actorRole === UserRole.SuperAdmin) {
    return [UserRole.Admin]
  }

  if (actorRole === UserRole.Admin) {
    return [UserRole.Admin, UserRole.Friend]
  }

  return []
}

export function canAccessCreateUserPage(role: UserRole | null): boolean {
  return role === UserRole.SuperAdmin || role === UserRole.Admin
}

export function canViewComics(role: UserRole | null): boolean {
  return role === UserRole.SuperAdmin || role === UserRole.Admin || role === UserRole.Friend
}

export function canManageComics(role: UserRole | null): boolean {
  return role === UserRole.Admin
}

export function canCreateRole(actorRole: UserRole | null, targetRole: UserRole): boolean {
  return getCreatableRolesByActor(actorRole).includes(targetRole)
}
